using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Gui.Models;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Library.UserDataLibrary.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.ViewModels.Cards
{
    public class DDCVHomeScenePermanentCardVM : DDCVHomeSceneCardVMBase
    {
        public DDCVHomeScenePermanentCardVM()
        {
            InitializeDashboard(DDCCPoolType.Permanent, 90, 90);
            softPityThreshold = 72;
            brLuckyLimit = 50;
            rndLuckyLimit = 50;
            rndUnluckyLimit = 72;
        }
        public override void RefreshGlobalDashboard()
        {
            SetDBVRate(DBVGlobalR5, GlobalRank5, GlobalTotal, 1.1, 2.1);
            SetDBVRate(DBVGlobalR4, GlobalRank4, GlobalTotal, 8, 18);
            if (CurrentBasicRoundCount > softPityThreshold)
            {
                SoftPityActivated = true;
                SoftPityChance = 0.006 + (CurrentBasicRoundCount - softPityThreshold) * 0.06;
            }
            else
            {
                SoftPityActivated = false;
                SoftPityChance = 0.006;
            }
        }
    }
}
