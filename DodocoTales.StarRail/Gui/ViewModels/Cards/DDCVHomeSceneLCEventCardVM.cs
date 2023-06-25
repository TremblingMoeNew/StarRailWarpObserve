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
            InitializeDashboard(DDCCPoolType.LCEvent, 160);
        }
        public override void RefreshGlobalDashboard()
        {
            SetDBVRate(DBVGlobalR5, GlobalRank5, GlobalTotal, 1.25, 2.45);
            SetDBVRate(DBVGlobalR5Up, GlobalRank5Up, GlobalTotal, 0.6, 2.38);
            SetDBVRate(DBVGlobalR4, GlobalRank4, GlobalTotal, 9, 19.5);
            SetDBVRate(DBVGlobalR4Up, GlobalRank4Up, GlobalTotal, 5, 18.8);
            if (currentBasicRountCount > 66)
            {
                SoftPityActivated = true;
                SoftPityChance = 0.008 + (currentBasicRountCount - 66) * 0.112 + (currentBasicRountCount>70 ? (currentBasicRountCount - 70) * -0.056 :0);
                SoftPityChance *= (CurrentRoundCurrent == currentBasicRountCount) ? 0.75 : 1;
            }
            else
            {
                SoftPityActivated = false;
            }
        }
    }
}
