using DodocoTales.SR.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.ViewModels.Cards
{
    public class DDCVHomeSceneCharEventCardVM : DDCVHomeSceneCardVMBase
    {
        public DDCVHomeSceneCharEventCardVM()
        {
            InitializeDashboard(DDCCPoolType.CharacterEvent, 180);
        }
        public override void RefreshGlobalDashboard()
        {
            SetDBVRate(DBVGlobalR5, GlobalRank5, GlobalTotal, 1.1, 2.1);
            SetDBVRate(DBVGlobalR5Up, GlobalRank5Up, GlobalTotal, 0.5, 1.6);
            SetDBVRate(DBVGlobalR4, GlobalRank4, GlobalTotal, 8, 18);
            SetDBVRate(DBVGlobalR4Up, GlobalRank4Up, GlobalTotal, 3.6, 13.6);
            if (currentBasicRountCount > 72)
            {
                SoftPityActivated = true;
                SoftPityChance = 0.006 + (currentBasicRountCount - 72) * 0.06;
            }
            else
            {
                SoftPityActivated = false;
            }
        }
    }
}
