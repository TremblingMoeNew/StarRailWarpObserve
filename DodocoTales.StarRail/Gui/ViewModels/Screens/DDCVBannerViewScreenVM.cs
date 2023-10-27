using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Gui.Enums;
using DodocoTales.SR.Gui.Models;
using DodocoTales.SR.Library;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DodocoTales.SR.Gui.ViewModels.Screens
{
    public class DDCVBannerViewScreenVM : ObservableObject
    {
        public ulong VersionID { get; set; }
        public ulong BannerInternalID { get; set; }
        public void SetBanner(ulong versionid, ulong bannerinternalid)
        {
            VersionID = versionid;
            BannerInternalID = bannerinternalid;
        }

        #region Title

        private string bannerName;
        public string BannerName
        {
            get => bannerName;
            set => SetProperty(ref bannerName, value);
        }

        private string version;
        public string Version
        {
            get => version;
            set => SetProperty(ref version, value);
        }

        private string versionName;
        public string VersionName
        {
            get => versionName;
            set => SetProperty(ref versionName, value);
        }

        private DDCCPoolType poolType;
        public DDCCPoolType PoolType
        {
            get => poolType;
            set
            {
                SetProperty(ref poolType, value);
                IsEventPool = (PoolType == DDCCPoolType.CharacterEvent || PoolType == DDCCPoolType.LCEvent);
            }
        }

        private bool isEventPool;
        public bool IsEventPool { get => isEventPool; set => SetProperty(ref isEventPool, value); }

        private DateTimeOffset beginTime;
        public DateTimeOffset BeginTime
        {
            get => beginTime;
            set => SetProperty(ref beginTime, value);
        }
        private DateTimeOffset endTime;
        public DateTimeOffset EndTime
        {
            get => endTime;
            set => SetProperty(ref endTime, value);
        }


        #endregion

        #region General
        private int total;
        public int Total { get => total; set => SetProperty(ref total, value); }

        private int totalIncludesInherited;
        public int TotalIncludesInherited { get => totalIncludesInherited; set => SetProperty(ref totalIncludesInherited, value); }

        private int rank5;
        public int Rank5 { get => rank5; set => SetProperty(ref rank5, value); }

        private int rank5IncludesInherited;
        public int Rank5IncludesInherited { get => rank5IncludesInherited; set => SetProperty(ref rank5IncludesInherited, value); }

        private int rank4;
        public int Rank4 { get => rank4; set => SetProperty(ref rank4, value); }

        private int rank5Up;
        public int Rank5Up { get => rank5Up; set => SetProperty(ref rank5Up, value); }

        private int rank4Up;
        public int Rank4Up { get => rank4Up; set => SetProperty(ref rank4Up, value); }


        private ObservableCollection<DDCVUnitIndicatorModel> rank5Ups;
        public ObservableCollection<DDCVUnitIndicatorModel> Rank5Ups
        {
            get => rank5Ups;
            set => SetProperty(ref rank5Ups, value);
        }
        private ObservableCollection<DDCVUnitIndicatorModel> rank4Ups;
        public ObservableCollection<DDCVUnitIndicatorModel> Rank4Ups
        {
            get => rank4Ups;
            set => SetProperty(ref rank4Ups, value);
        }


        #endregion

        #region Dashboard
        private List<PieSeries<ObservableValue>> seriesGlobalR5;
        public List<PieSeries<ObservableValue>> SeriesGlobalR5
        {
            get => seriesGlobalR5;
            set => SetProperty(ref seriesGlobalR5, value);
        }
        public ObservableValue DBVGlobalR5 = new ObservableValue(0.5);
        public ObservableValue DBVGlobalR5Up = new ObservableValue(0.5);

        private List<PieSeries<ObservableValue>> seriesGlobalR4;
        public List<PieSeries<ObservableValue>> SeriesGlobalR4
        {
            get => seriesGlobalR4;
            set => SetProperty(ref seriesGlobalR4, value);
        }
        public ObservableValue DBVGlobalR4 = new ObservableValue(0.5);
        public ObservableValue DBVGlobalR4Up = new ObservableValue(0.5);


        public void SetDBVRate(ObservableValue ov, int divident, int divisor, double lowerlimit, double upperlimit)
        {
            if (divisor == 0)
            {
                ov.Value = 0;
                return;
            }
            var rate = divident * 100.0 / divisor;
            if (rate < lowerlimit)
            {
                ov.Value = 0;
            }
            else if (rate > upperlimit)
            {
                ov.Value = 1;
            }
            else
            {
                ov.Value = (rate - lowerlimit) / (upperlimit - lowerlimit);
            }
        }
        public void InitializeDashboard()
        {
            SeriesGlobalR5 = new GaugeBuilder()
                .WithInnerRadius(36)
                .WithBackgroundInnerRadius(36)
                .AddValue(DBVGlobalR5Up, "", SKColor.Parse("#D0BA6FD9"))
                .AddValue(DBVGlobalR5, "", SKColor.Parse("#048CFF"))
                .WithLabelsSize(0)
                .BuildSeries();
            foreach (var ser in SeriesGlobalR5)
            {
                ser.IsHoverable = false;
            }
            SeriesGlobalR4 = new GaugeBuilder()
                .WithInnerRadius(36)
                .WithBackgroundInnerRadius(36)
                .AddValue(DBVGlobalR4Up, "", SKColor.Parse("#D0BA6FD9"))
                .AddValue(DBVGlobalR4, "", SKColor.Parse("#048CFF"))
                .WithLabelsSize(0)
                .BuildSeries();
            foreach (var ser in SeriesGlobalR4)
            {
                ser.IsHoverable = false;
            }
        }

        public void RefreshDashboard()
        {
            switch (PoolType)
            {
                case DDCCPoolType.CharacterEvent:
                    SetDBVRate(DBVGlobalR5, Rank5IncludesInherited, TotalIncludesInherited, 1.1, 2.1);
                    SetDBVRate(DBVGlobalR5Up, Rank5Up, TotalIncludesInherited, 0.5, 1.6);
                    SetDBVRate(DBVGlobalR4, Rank4, Total, 8, 18);
                    SetDBVRate(DBVGlobalR4Up, Rank4Up, Total, 3.6, 13.6);
                    break;
                case DDCCPoolType.LCEvent:
                    SetDBVRate(DBVGlobalR5, Rank5IncludesInherited, TotalIncludesInherited, 1.25, 2.45);
                    SetDBVRate(DBVGlobalR5Up, Rank5Up, TotalIncludesInherited, 0.6, 2.38);
                    SetDBVRate(DBVGlobalR4, Rank4, Total, 9, 19.5);
                    SetDBVRate(DBVGlobalR4Up, Rank4Up, Total, 5, 18.8);
                    break;
                case DDCCPoolType.Beginner:
                    SetDBVRate(DBVGlobalR5, Rank5IncludesInherited, TotalIncludesInherited, 0.5, 3.5);
                    SetDBVRate(DBVGlobalR5Up, 0, TotalIncludesInherited, 0.5, 3.5);
                    SetDBVRate(DBVGlobalR4, Rank4, Total, 8, 18);
                    SetDBVRate(DBVGlobalR4Up, 0, Total, 8, 18);
                    break;
                case DDCCPoolType.Permanent:
                default:
                    SetDBVRate(DBVGlobalR5, Rank5IncludesInherited, TotalIncludesInherited, 1.1, 2.1);
                    SetDBVRate(DBVGlobalR5Up, 0, TotalIncludesInherited, 1.1, 2.1);
                    SetDBVRate(DBVGlobalR4, Rank4, Total, 8, 18);
                    SetDBVRate(DBVGlobalR4Up, 0, Total, 8, 18);
                    break;
            }

        }



        #endregion

        #region RoundList

        private ObservableCollection<DDCVRoundItemModel> rounds;
        public ObservableCollection<DDCVRoundItemModel> Rounds
        {
            get => rounds;
            set
            {
                SetProperty(ref rounds, value);
                RoundCnt = Rounds.Count;
            }
        }
        private int roundCnt;
        public int RoundCnt { get => roundCnt; set => SetProperty(ref roundCnt, value); }

        #endregion

        #region SelectedRound
        private DDCVRoundItemModel selectedRound;
        public DDCVRoundItemModel SelectedRound
        {
            get => selectedRound;
            set
            {
                if (value != selectedRound)
                {
                    SetProperty(ref selectedRound, value);
                    UpdateSelectedRound();
                }

            }
        }

        private ObservableCollection<DDCVUnitIndicatorModel> rank5InRound;
        public ObservableCollection<DDCVUnitIndicatorModel> Rank5InRound
        {
            get => rank5InRound;
            set => SetProperty(ref rank5InRound, value);
        }
        private ObservableCollection<DDCVUnitIndicatorModel> rank4InRound;
        public ObservableCollection<DDCVUnitIndicatorModel> Rank4InRound
        {
            get => rank4InRound;
            set => SetProperty(ref rank4InRound, value);
        }


        public void UpdateSelectedRound()
        {
            if (SelectedRound == null || SelectedRound == DependencyProperty.UnsetValue) return;
            var tz = DDCL.CurrentUser.GetActivatingTimeZone();

            var r5s = new List<DDCVUnitIndicatorModel>();
            foreach (var rnd in SelectedRound.LogItem.Logs.GroupBy(x => x.RoundID))
            {
                var item = rnd.LastOrDefault();
                if (item == null) continue;
                if (item.Rank != 5) continue;
                var ver = DDCL.BannerLib.GetVersion(item.VersionID);
                var ban = DDCL.BannerLib.GetBanner(item.BannerInternalID);
                r5s.Add(new DDCVUnitIndicatorModel
                {
                    Name = item.Name,
                    Count = rnd.Count(),
                    Version = ver.Version,
                    Banner = ban.Name,
                    Time = DDCL.GetTimeOffset(item.Time, tz)
                });
            }
            Rank5InRound = new ObservableCollection<DDCVUnitIndicatorModel>(r5s);

            var r4s = new List<DDCVUnitIndicatorModel>();
            foreach (var item in SelectedRound.LogItem.Logs.FindAll(x => x.Rank == 4))
            {
                var ver = DDCL.BannerLib.GetVersion(item.VersionID);
                var ban = DDCL.BannerLib.GetBanner(item.BannerInternalID);
                r4s.Add(new DDCVUnitIndicatorModel
                {
                    Name = item.Name,
                    Version = ver.Version,
                    Banner = ban.Name,
                    Time = DDCL.GetTimeOffset(item.Time, tz)
                });
            }
            Rank4InRound = new ObservableCollection<DDCVUnitIndicatorModel>(r4s);

            foreach (var item in GridIndicatorModels)
            {
                item.IndicatorType = DDCVUnitIndicatorType.Default;
                item.Inherited = false;
            }

            switch (SelectedRound.LogItem.CategorizedGachaType)
            {
                case DDCCPoolType.Beginner:
                    GreaterRoundType = DDCVGreaterRoundType.Beginner;
                    break;
                case DDCCPoolType.Permanent:
                    GreaterRoundType = DDCVGreaterRoundType.Permanent;
                    break;
                case DDCCPoolType.CharacterEvent:
                default:
                    GreaterRoundType = DDCVGreaterRoundType.CharacterEvent;
                    break;
                case DDCCPoolType.LCEvent:
                    GreaterRoundType = DDCVGreaterRoundType.LCEvent;
                    break;
            }

            for (int i = 0; i < SelectedRound.LogItem.Logs.Count; i++)
            {
                var item = SelectedRound.LogItem.Logs[i];
                var model = GridIndicatorModels[i];

                var ver = DDCL.BannerLib.GetVersion(item.VersionID);
                var ban = DDCL.BannerLib.GetBanner(item.BannerInternalID);

                model.Name = item.Name;
                model.Version = ver.Version;
                model.Banner = ban.Name;
                model.Time = DDCL.GetTimeOffset(item.Time, tz);
                model.Inherited = !(item.BannerInternalID == BannerInternalID);
                if (item.Rank == 3)
                {
                    model.IndicatorType = DDCVUnitIndicatorType.Rank3;
                }
                else if (item.Rank == 4)
                {
                    if (ban.Rank4Up.Contains(item.Name))
                        model.IndicatorType = DDCVUnitIndicatorType.Rank4Up;
                    else if (item.UnitType == DDCCUnitType.Character)
                        model.IndicatorType = DDCVUnitIndicatorType.Rank4PermanentChar;
                    else
                        model.IndicatorType = DDCVUnitIndicatorType.Rank4PermanentLC;
                }
                else
                {
                    if (ban.Rank5Up.Contains(item.Name))
                    {
                        model.IndicatorType = DDCVUnitIndicatorType.Rank5Up;
                    }
                    else if (item.UnitType == DDCCUnitType.Character)
                        model.IndicatorType = DDCVUnitIndicatorType.Rank5PermanentChar;
                    else
                        model.IndicatorType = DDCVUnitIndicatorType.Rank5PermanentLC;
                }
                if(item.Imported)
                {
                    model.Imported = true;
                    model.Untrusted = item.Untrusted;
                    var app = DDCL.ExportersLib.GetExporter(item.ImportApplication);
                    model.ImportApplication = $"{app.ApplicationNameChinese}({app.Author})";
                }

            }
        }

        #endregion

        #region SelectedRound_GridIndicator
        private ObservableCollection<ObservableCollection<DDCVUnitIndicatorModel>> gridIndicatorItems;
        public ObservableCollection<ObservableCollection<DDCVUnitIndicatorModel>> GridIndicatorItems
        {
            get => gridIndicatorItems;
            set => SetProperty(ref gridIndicatorItems, value);
        }

        private List<DDCVUnitIndicatorModel> gridIndicatorModels;
        public List<DDCVUnitIndicatorModel> GridIndicatorModels
        {
            get => gridIndicatorModels;
            set => SetProperty(ref gridIndicatorModels, value);
        }

        private DDCVGreaterRoundType greaterRoundType;
        public DDCVGreaterRoundType GreaterRoundType
        {
            get => greaterRoundType;
            set
            {
                if (value != greaterRoundType)
                {
                    SetProperty(ref greaterRoundType, value);
                    ResizeGridIndicator();
                }
            }
        }

        public void ResizeGridIndicator()
        {
            int row = 0, col = 0;
            switch (GreaterRoundType)
            {
                case DDCVGreaterRoundType.Beginner:
                    row = 2;
                    col = 25;
                    break;
                case DDCVGreaterRoundType.Permanent:
                    row = 3;
                    col = 30;
                    break;
                case DDCVGreaterRoundType.CharacterEvent:
                    row = 5;
                    col = 36;
                    break;
                case DDCVGreaterRoundType.LCEvent:
                    row = 5;
                    col = 32;
                    break;
            }
            GridIndicatorItems.Clear();
            for (int i = 0; i < col; i++)
            {
                var ls = new ObservableCollection<DDCVUnitIndicatorModel>();
                for (int j = 0; j < row; j++)
                {
                    ls.Add(GridIndicatorModels[i * row + j]);
                }
                GridIndicatorItems.Add(ls);
            }
        }
        public void InitializeGridIndicator()
        {
            GridIndicatorItems = new ObservableCollection<ObservableCollection<DDCVUnitIndicatorModel>>();
            GridIndicatorModels = new List<DDCVUnitIndicatorModel>();
            for (int i = 0; i < 320; i++)
                GridIndicatorModels.Add(new DDCVUnitIndicatorModel { Index = i + 1 });
            GreaterRoundType = DDCVGreaterRoundType.Permanent;
        }

        #endregion



        public DDCVBannerViewScreenVM()
        {
            InitializeDashboard();
            InitializeGridIndicator();
        }

        public void ReloadData()
        {
            var version = DDCL.BannerLib.GetVersion(VersionID);
            Version = version.Version;
            VersionName = version.Name;

            var banner = DDCL.BannerLib.GetBanner(BannerInternalID);
            BannerName = banner.Name;
            PoolType = banner.Type;

            var tz = DDCL.CurrentUser.GetActivatingTimeZone();
            BeginTime = DDCL.GetBannerTimeOffset(banner.BeginTime, banner.BeginTimeSync, tz).ToLocalTime();
            EndTime = DDCL.GetBannerTimeOffset(banner.EndTime, banner.EndTimeSync, tz).ToLocalTime();

            var bannerlog = DDCL.CurrentUser.GetBanner(BannerInternalID);
            Total = bannerlog.Logs.Count;
            Rank5 = bannerlog.Logs.Count(x => x.Rank == 5);
            Rank5Up = bannerlog.Logs.Count(x => banner.Rank5Up.Contains(x.Name));
            Rank4 = bannerlog.Logs.Count(x => x.Rank == 4);
            Rank4Up = bannerlog.Logs.Count(x => banner.Rank4Up.Contains(x.Name));
            TotalIncludesInherited = Total +
                bannerlog.GreaterRounds.First()?.Logs.Count(x => x.BannerInternalID != BannerInternalID) ?? 0;
            Rank5IncludesInherited = Rank5 +
                bannerlog.GreaterRounds.First()?.Logs.Count(
                    x => x.Rank == 5 && (x.BannerInternalID != BannerInternalID)
                ) ?? 0;

            var r5ups = new List<DDCVUnitIndicatorModel>();
            var r4ups = new List<DDCVUnitIndicatorModel>();

            if (IsEventPool)
            {
                foreach (var r5 in banner.Rank5Up)
                {
                    r5ups.Add(new DDCVUnitIndicatorModel
                    {
                        Name = r5,
                        Count = bannerlog.Logs.Count(x => x.Name == r5)
                    });
                }
                r5ups.Add(new DDCVUnitIndicatorModel
                {
                    Name = "常驻",
                    Count = Rank5 - Rank5Up
                });
                foreach (var r4 in banner.Rank4Up)
                {
                    r4ups.Add(new DDCVUnitIndicatorModel
                    {
                        Name = r4,
                        Count = bannerlog.Logs.Count(x => x.Name == r4)
                    });
                }
                r4ups.Add(new DDCVUnitIndicatorModel
                {
                    Name = "常驻",
                    Count = Rank4 - Rank4Up
                });
            }
            else
            {
                var r5chr = bannerlog.Logs.Count(x => x.Rank == 5 && x.UnitType == DDCCUnitType.Character);
                var r4chr = bannerlog.Logs.Count(x => x.Rank == 4 && x.UnitType == DDCCUnitType.Character);
                r5ups.Add(new DDCVUnitIndicatorModel
                {
                    Name = "常驻角色",
                    Count = r5chr
                });
                r5ups.Add(new DDCVUnitIndicatorModel
                {
                    Name = "常驻武器",
                    Count = Rank5 - r5chr
                });
                r4ups.Add(new DDCVUnitIndicatorModel
                {
                    Name = "常驻角色",
                    Count = r4chr
                });
                r4ups.Add(new DDCVUnitIndicatorModel
                {
                    Name = "常驻武器",
                    Count = Rank4 - r4chr
                });
            }
            Rank5Ups = new ObservableCollection<DDCVUnitIndicatorModel>(r5ups);
            Rank4Ups = new ObservableCollection<DDCVUnitIndicatorModel>(r4ups);


            var rnds = new List<DDCVRoundItemModel>();
            var idx = 1;
            foreach (var r in bannerlog.GreaterRounds)
            {
                if (r.CategorizedGachaType == DDCCPoolType.Beginner && r.Logs.Count == 0) continue;
                rnds.Add(new DDCVRoundItemModel
                {
                    Index = idx++,
                    VersionID = VersionID,
                    BannerInternalID = BannerInternalID,
                    LogItem = r
                });
            }
            rnds.Reverse();
            SelectedRound = rnds.First();
            Rounds = new ObservableCollection<DDCVRoundItemModel>(rnds);

            RefreshDashboard();
        }
    }
}
