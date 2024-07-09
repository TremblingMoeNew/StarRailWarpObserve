using DodocoTales.SR.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace DodocoTales.SR.Common.Services
{
    public static class DDCCChanceProvider
    {
        public static readonly double CharEventUpRatio = 0.5625;
        public static readonly double LCEventUpRatio = 0.78125;

        public static readonly double PermanentBasicRatio = 0.006;
        public static readonly int PermanentSoftPityThreshold = 72;
        public static readonly double PermanentSoftPityRatio = 0.06;

        public static readonly double LCEventBasicRatio = 0.008;
        public static readonly int LCEventSoftPityThreshold = 64;
        public static readonly double LCEventSoftPityRatio = 0.07;

        public static double GetNextPullRank5Chance(DDCCPoolType type, int count)
        {
            double bRatio = PermanentBasicRatio;
            double spRatio = PermanentSoftPityRatio;
            int spThreshold = PermanentSoftPityThreshold;

            if (type == DDCCPoolType.LCEvent)
            {
                bRatio = LCEventBasicRatio;
                spRatio = LCEventSoftPityRatio;
                spThreshold = LCEventSoftPityThreshold;
            }
            return Math.Min(bRatio + Math.Max(count - spThreshold, 0) * spRatio, 1);
        }

        public static double GetNextPullTargetChance(DDCCPoolType type, int count, bool previous_permanent)
        {
            double upRatio = 1;

            switch (type)
            {
                case DDCCPoolType.CharacterEvent:
                    if (!previous_permanent) { upRatio = CharEventUpRatio; };
                    break;
                case DDCCPoolType.LCEvent:
                    if (!previous_permanent) { upRatio = LCEventUpRatio; };
                    break;
            }
            return GetNextPullRank5Chance(type, count) * upRatio;
        }

        public static double GetNextPullPermanantChance(DDCCPoolType type, int count, bool previous_permanent = false)
        {
            double upRatio = 1;

            switch (type)
            {
                case DDCCPoolType.CharacterEvent:
                    if (!previous_permanent) { upRatio = CharEventUpRatio; };
                    break;
                case DDCCPoolType.LCEvent:
                    if (!previous_permanent) { upRatio = LCEventUpRatio; };
                    break;
            }
            return GetNextPullRank5Chance(type, count) * (1 - upRatio);
        }


        public static double GetNextPullNoR5Chance(DDCCPoolType type, int count)
        {
            return 1 - GetNextPullRank5Chance(type, count);
        }
    }
}
