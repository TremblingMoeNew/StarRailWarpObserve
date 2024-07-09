using DodocoTales.SR.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.ViewModels.Cards
{
    public class DDCVHomeSceneLCEventCardVM : DDCVHomeSceneCardVMBase
    {
        public DDCVHomeSceneLCEventCardVM()
        {
            InitializeDashboard(DDCCPoolType.LCEvent, 160, 80);
            softPityThreshold = 65;
            brLuckyLimit = 48;
            rndLuckyLimit = 50;
            rndUnluckyLimit = 80;
        }
        public override void RefreshGlobalDashboard()
        {
            SetDBVRate(DBVGlobalR5, GlobalRank5, GlobalTotal, 1.25, 2.45);
            SetDBVRate(DBVGlobalR5Up, GlobalRank5Up, GlobalTotal, 0.6, 2.38);
            SetDBVRate(DBVGlobalR4, GlobalRank4, GlobalTotal, 9, 19.5);
            SetDBVRate(DBVGlobalR4Up, GlobalRank4Up, GlobalTotal, 5, 18.8);
            if (CurrentBasicRoundCount > softPityThreshold)
            {
                SoftPityActivated = true;
                SoftPityChance = Math.Max(0.008 + (CurrentBasicRoundCount - softPityThreshold) * 0.07, 1);
                SoftPityChance *= (CurrentRoundCurrent == CurrentBasicRoundCount) ? 0.78125 : 1;
            }
            else
            {
                SoftPityActivated = false;
                SoftPityChance = 0.008;
                SoftPityChance *= (CurrentRoundCurrent == CurrentBasicRoundCount) ? 0.78125 : 1;
            }
        }
    }
}
