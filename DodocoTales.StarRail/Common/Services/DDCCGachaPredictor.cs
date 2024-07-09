using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Common.Services
{
    public class DDCCGachaPredictor
    {
        public List<DDCCPredictTargetItem> targets = new List<DDCCPredictTargetItem>();

        public decimal[,,,] ChanceMatrix { get; set; }

        public decimal[] ChancesByTargetCount { get; set; }
        public int Total { get; set; }
        int ResultIndex;

        public DDCCGachaPredictor AddTarget(DDCCPoolType type, int previous_count=0, bool previous_permanant=false)
        {
            targets.Add(new DDCCPredictTargetItem
            {
                PoolType = type,
                PreviousCount = previous_count,
                IsPreviousPermanant = previous_permanant,
            });
            return this;
        }

        public DDCCGachaPredictor Generate(int total)
        {
            int targetCnt = targets.Count;
            Total = total;
            ResultIndex = Total % 2;
            int tmax = Math.Max(Total + 1, 91);

            if (targetCnt == 0)
            {
                ChanceMatrix = null;
                ChancesByTargetCount = null;
                return this;
            }
            ChanceMatrix = new decimal[2, targetCnt + 1, 2, tmax];

            ChanceMatrix[0, 0, (targets[0].IsPreviousPermanant ? 1 : 0), targets[0].PreviousCount] = 1M;

            for (int pull = 0; pull < total; pull++)
            {
                // For target r5 cnt not matched
                for (int upc = 0; upc < targetCnt; upc++)
                {
                    for (int current = 0; current < 90; current++)
                    {
                        int thisregion = pull % 2, nextregion = (pull + 1) % 2;

                        // For [x,x,0,x]
                        decimal thisvalue = ChanceMatrix[thisregion, upc, 0, current];
                        ChanceMatrix[nextregion, upc + 1, 0, (upc + 1 < targetCnt ? (targets[upc + 1].PreviousCount) : 0)]
                            += (decimal)DDCCChanceProvider.GetNextPullTargetChance(targets[upc].PoolType, current, false) * thisvalue;
                        ChanceMatrix[nextregion, upc, 1, 0]
                            += (decimal)DDCCChanceProvider.GetNextPullPermanantChance(targets[upc].PoolType, current) * thisvalue;
                        ChanceMatrix[nextregion, upc, 0, current + 1]
                           += (decimal)DDCCChanceProvider.GetNextPullNoR5Chance(targets[upc].PoolType, current) * thisvalue;

                        // For [x,x,1,x]
                        thisvalue = ChanceMatrix[thisregion, upc, 1, current];
                        ChanceMatrix[nextregion, upc + 1, 0, (upc + 1 < targetCnt ? (targets[upc + 1].PreviousCount) : 0)]
                            += (decimal)DDCCChanceProvider.GetNextPullTargetChance(targets[upc].PoolType, current, true) * thisvalue;
                        ChanceMatrix[nextregion, upc, 1, current + 1]
                           += (decimal)DDCCChanceProvider.GetNextPullNoR5Chance(targets[upc].PoolType, current) * thisvalue;
                        ChanceMatrix[thisregion, upc, 0, current] = 0;
                        ChanceMatrix[thisregion, upc, 1, current] = 0;
                    }
                }
                // For target r5 cnt matched
                for (int current = 0; current < Total; current++)
                {
                    int thisregion = pull % 2, nextregion = (pull + 1) % 2;
                    ChanceMatrix[nextregion, targetCnt, 0, current + 1] += ChanceMatrix[thisregion, targetCnt, 0, current];
                    ChanceMatrix[thisregion, targetCnt, 0, current] = 0;
                }
            }

            ChancesByTargetCount = new decimal[targetCnt + 1];

            for (int i = 0; i <= targetCnt; i++)
            {
                for (int j = 0; j < 2;  j++)
                {
                    for (int k = 0; k < tmax; k++)
                    {
                        ChancesByTargetCount[i] += ChanceMatrix[ResultIndex, i, j, k];
                    }
                }
            }
            return this;
        }
    }
}
