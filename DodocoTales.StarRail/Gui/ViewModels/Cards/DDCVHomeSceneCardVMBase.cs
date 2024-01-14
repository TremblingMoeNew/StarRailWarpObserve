using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Gui.Models;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.CurrentUser.Models;
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
    public class DDCVHomeSceneCardVMBase : ObservableObject
    {
        private bool softPityActivated;
        public bool SoftPityActivated
        {
            get => softPityActivated;
            set => SetProperty(ref softPityActivated, value);
        }

        protected int softPityThreshold;
        protected int brLuckyLimit;
        protected int rndLuckyLimit;
        protected int rndUnluckyLimit;

        public double softPityChance;
        public double SoftPityChance
        {
            get => softPityChance;
            set => SetProperty(ref softPityChance, value);
        }

        #region Properties_CurrentBanner_Title

        private string version;
        public string Version
        {
            get => version;
            set => SetProperty(ref version, value);
        }
        private string bannerTitle;
        public string BannerTitle
        {
            get => bannerTitle;
            set => SetProperty(ref bannerTitle, value);
        }

        private string upUnits;
        public string UpUnits
        {
            get => upUnits;
            set => SetProperty(ref upUnits, value);
        }

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

        private bool currentBannerExists;
        public bool CurrentBannerExists
        {
            get => currentBannerExists;
            set => SetProperty(ref currentBannerExists, value);
        }

        private ObservableCollection<string> upRank5s;
        public ObservableCollection<string> UpRank5s
        {
            get => upRank5s;
            set => SetProperty(ref upRank5s, value);
        }
        private ObservableCollection<string> upRank4s;
        public ObservableCollection<string> UpRank4s
        {
            get => upRank4s;
            set => SetProperty(ref upRank4s, value);
        }
        #endregion

        #region Properties_Global

        private int globalTotal;
        public int GlobalTotal
        {
            get => globalTotal;
            set => SetProperty(ref globalTotal, value);
        }
        private int globalRank5;
        public int GlobalRank5
        {
            get => globalRank5;
            set => SetProperty(ref globalRank5, value);
        }
        private int globalRank5Up;
        public int GlobalRank5Up
        {
            get => globalRank5Up;
            set => SetProperty(ref globalRank5Up, value);
        }
        private int globalRank4;
        public int GlobalRank4
        {
            get => globalRank4;
            set => SetProperty(ref globalRank4, value);
        }
        private int globalRank4Up;
        public int GlobalRank4Up
        {
            get => globalRank4Up;
            set => SetProperty(ref globalRank4Up, value);
        }
        private int globalRank3;
        public int GlobalRank3
        {
            get => globalRank3;
            set => SetProperty(ref globalRank3, value);
        }

        private ObservableCollection<DDCVUnitIndicatorModel> rank5List = new ObservableCollection<DDCVUnitIndicatorModel>();
        public ObservableCollection<DDCVUnitIndicatorModel> Rank5List
        {
            get => rank5List;
            set => SetProperty(ref rank5List, value);
        }
        private ObservableCollection<DDCVUnitIndicatorModel> rank5UpList = new ObservableCollection<DDCVUnitIndicatorModel>();
        public ObservableCollection<DDCVUnitIndicatorModel> Rank5UpList
        {
            get => rank5UpList;
            set => SetProperty(ref rank5UpList, value);
        }
        #endregion

        #region Properties_Current
        private int currentTotal;
        public int CurrentTotal
        {
            get => currentTotal;
            set => SetProperty(ref currentTotal, value);
        }
        private int currentRank5;
        public int CurrentRank5
        {
            get => currentRank5;
            set => SetProperty(ref currentRank5, value);
        }
        private int currentRank5Up;
        public int CurrentRank5Up
        {
            get => currentRank5Up;
            set => SetProperty(ref currentRank5Up, value);
        }
        private int currentRank4;
        public int CurrentRank4
        {
            get => currentRank4;
            set => SetProperty(ref currentRank4, value);
        }
        private int currentRank4Up;
        public int CurrentRank4Up
        {
            get => currentRank4Up;
            set => SetProperty(ref currentRank4Up, value);
        }
        private int currentRank3;
        public int CurrentRank3
        {
            get => currentRank3;
            set => SetProperty(ref currentRank3, value);
        }
        #endregion


        #region Properties_Dashboard
        private List<PieSeries<ObservableValue>> seriesGlobalR5;
        public List<PieSeries<ObservableValue>> SeriesGlobalR5
        {
            get => seriesGlobalR5;
            set => SetProperty(ref seriesGlobalR5, value);
        }
        public ObservableValue DBVGlobalR5 = new ObservableValue(0);
        public ObservableValue DBVGlobalR5Up = new ObservableValue(0);
        private List<PieSeries<ObservableValue>> seriesGlobalR4;
        public List<PieSeries<ObservableValue>> SeriesGlobalR4
        {
            get => seriesGlobalR4;
            set => SetProperty(ref seriesGlobalR4, value);
        }
        public ObservableValue DBVGlobalR4 = new ObservableValue(0);
        public ObservableValue DBVGlobalR4Up = new ObservableValue(0);

        private ISeries[] seriesCurrentRound;
        public ISeries[] SeriesCurrentRound
        {
            get => seriesCurrentRound;
            set => SetProperty(ref seriesCurrentRound, value);
        }

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

        public ObservableValue DBVPBannerPermanent = new ObservableValue(0);
        public ObservableValue DBVPBannerUnaligned = new ObservableValue(0);
        public ObservableValue DBVCBannerPermanent = new ObservableValue(0);
        public ObservableValue DBVCurrent = new ObservableValue(0);
        public ObservableValue DBVRemains = new ObservableValue(160);
        public ObservableValue DBVUnavailable = new ObservableValue(0);

        private int currentRoundIndex;
        public int CurrentRoundIndex
        {
            get => currentRoundIndex;
            set => SetProperty(ref currentRoundIndex, value);
        }

        private int currentRoundCurrent;
        public int CurrentRoundCurrent
        {
            get => currentRoundCurrent;
            set => SetProperty(ref currentRoundCurrent, value);
        }

        private int currentRoundTotal;
        public int CurrentRoundTotal
        {
            get => currentRoundTotal;
            set
            {
                SetProperty(ref currentRoundTotal, value);
                RefreshCurrentRoundRemains();
            }
        }

        private int currentBasicRoundCount;
        public int CurrentBasicRoundCount
        {
            get => currentBasicRoundCount;
            set => SetProperty(ref currentBasicRoundCount, value);
        }

        private int basicRoundTotal;
        public int BasicRoundTotal
        {
            get => basicRoundTotal;
            set
            {
                SetProperty(ref basicRoundTotal, value);
            }
        }

        public static PieSeries<ObservableValue> CreatePieSeries(ObservableValue value, string name, string color, double innerrradius = 50, bool hoverable = true)
        {
            return new PieSeries<ObservableValue> { Values = new ObservableValue[] { value }, InnerRadius = innerrradius, HoverPushout = 0, Name = name, Fill = new SolidColorPaint(SKColor.Parse(color)), IsHoverable = hoverable };
        }
        public void SetCurrentRoundValues(int pbp, int pbu, int cbp, int cur)
        {
            CurrentRoundCurrent = pbp + pbu + cbp + cur;
            CurrentBasicRoundCount = cur + (cbp > 0 ? 0 : pbu);

            DBVPBannerPermanent.Value = pbp;
            DBVPBannerUnaligned.Value = pbu;
            DBVCBannerPermanent.Value = cbp;
            DBVCurrent.Value = cur;
            DBVUnavailable.Value = (pbp > 0 || cbp > 0) ? BasicRoundTotal - (CurrentRoundCurrent - CurrentBasicRoundCount) : 0;

            RefreshCurrentRoundRemains();
        }
        public void RefreshCurrentRoundRemains()
        {
            DBVRemains.Value = CurrentRoundTotal - CurrentRoundCurrent - DBVUnavailable.Value;
        }

        public void InitializeDashboard(DDCCPoolType poolType, int roundTotal, int basicRound = 90)
        {
            SeriesGlobalR5 = new GaugeBuilder()
                .WithInnerRadius(32)
                .WithBackgroundInnerRadius(32)
                .AddValue(DBVGlobalR5Up, "", SKColor.Parse("#B0FC6500"))
                .AddValue(DBVGlobalR5, "", SKColor.Parse("#D0DF1000"))
                .WithLabelsSize(0)
                .BuildSeries();
            foreach (var ser in SeriesGlobalR5)
            {
                ser.IsHoverable = false;
            }
            SeriesGlobalR4 = new GaugeBuilder()
                .WithInnerRadius(32)
                .WithBackgroundInnerRadius(32)
                .AddValue(DBVGlobalR4Up, "", SKColor.Parse("#B08C46F5"))
                .AddValue(DBVGlobalR4, "", SKColor.Parse("#D0411BE2"))
                .WithLabelsSize(0)
                .BuildSeries();
            foreach (var ser in SeriesGlobalR4)
            {
                ser.IsHoverable = false;
            }
            SeriesCurrentRound = new ISeries[]
            {
                CreatePieSeries(DBVPBannerPermanent,"继承（常驻）","#B0AA8BC2",45),
                CreatePieSeries(DBVPBannerUnaligned,"继承（无五星）","#B0555555",45),
                CreatePieSeries(DBVCBannerPermanent,"本卡池（常驻）","#B0D67593",45),
                CreatePieSeries(DBVCurrent,"本卡池（当前）","#048CFF",45),
                CreatePieSeries(DBVRemains,"","#EEEEEE",45,false),
                CreatePieSeries(DBVUnavailable,"","#555555",45,false),
            };
            PoolType = poolType;
            CurrentRoundTotal = roundTotal;
            BasicRoundTotal = basicRound;
        }


        #endregion




        #region UnitIndicator
        private int unitIndicatorCurrentPageIndex;
        public int UnitIndicatorCurrentPageIndex
        {
            get => unitIndicatorCurrentPageIndex;
            set
            {
                SetProperty(ref unitIndicatorCurrentPageIndex, value);
            }
        }
        #endregion

        private DDCCPoolType PoolType;

        public DDCVHomeSceneCardVMBase()
        {
            Version = "---";
            BannerTitle = "---";
            CurrentBannerExists = false;
            UpRank4s = new ObservableCollection<string> { null, null, null };
            UpRank5s = new ObservableCollection<string> { null, null, null };
        }


        public void ReloadData()
        {
            LoadGlobal();
            LoadCurrentBanner();
            RefreshGlobalDashboard();
        }


        public void LoadGlobal()
        {
            var logs = DDCL.CurrentUser.Logs.Values.ToList().FindAll(x => x.PoolType == PoolType);
            GlobalTotal = logs.Count;
            GlobalRank5 = GlobalRank5Up = GlobalRank4 = GlobalRank4Up = GlobalRank3 = 0;
            foreach (var grp in logs.GroupBy(x => x.Rank))
            {
                var rank = grp.First().Rank;
                if (rank == 5)
                {
                    var r5 = grp.ToList();
                    GlobalRank5 = r5.Count;
                    int upc = 0;
                    foreach (var item in r5)
                    {
                        if (DDCL.BannerLib.GetBanner(item.BannerInternalID).Rank5Up.Contains(item.Name)) upc++;
                    }
                    GlobalRank5Up = upc;
                }
                if (rank == 4)
                {
                    var r4 = grp.ToList();
                    GlobalRank4 = r4.Count;
                    int upc = 0;
                    foreach (var item in r4)
                    {
                        if (DDCL.BannerLib.GetBanner(item.BannerInternalID).Rank4Up.Contains(item.Name)) upc++;
                    }
                    GlobalRank4Up = upc;
                }
                if (rank == 3)
                {
                    var r3 = grp.ToList();
                    GlobalRank3 = r3.Count;
                }
            }
            RebuildRank5List(logs);
        }


        public void LoadCurrentBanner()
        {
            DDCLBannerLogItem currentlog = null;
            var banners = DDCL.BannerLib.GetBannersByType(PoolType);
            var current = banners.FirstOrDefault(x => x.ActivateStatus == DDCLActivateStatus.Activating);
            if (current == null)
            {
                Version = "---";
                BannerTitle = "---";
                CurrentBannerExists = false;
                goto label_current_round;
            }
            Version = DDCL.BannerLib.GetVersion(current.VersionID).Version;
            BannerTitle = current.Name;

            string upunit = "";
            foreach (var r5up in current.Rank5Up)
            {
                upunit += string.Format("{0} ", r5up);
            }
            upunit += "|";
            foreach (var r4up in current.Rank4Up)
            {
                upunit += string.Format(" {0}", r4up);
            }
            UpUnits = upunit;

            var upr5 = new ObservableCollection<string>(current.Rank5Up);
            if (upr5.Count > 0)
            {
                if (upr5.Count < 3) upr5.Insert(1, null);
                if (upr5.Count < 3) upr5.Insert(0, null);
            }

            UpRank5s = upr5;

            var upr4 = new ObservableCollection<string>(current.Rank4Up);
            if (upr4.Count > 0) 
            {
                if (upr4.Count < 3) upr4.Insert(1, null);
                if (upr4.Count < 3) upr4.Insert(0, null);
            }
            
            UpRank4s = upr4;

            var tz = DDCL.CurrentUser.GetActivatingTimeZone();
            BeginTime = DDCL.GetBannerTimeOffset(current.BeginTime, current.BeginTimeSync, tz).ToLocalTime();
            EndTime = DDCL.GetBannerTimeOffset(current.EndTime, current.EndTimeSync, tz).ToLocalTime();
            CurrentBannerExists = true;


            currentlog = DDCL.CurrentUser.GetBanner(current.InternalID);
            if (currentlog == null)
            {
                goto label_current_round;
            }
            CurrentTotal = currentlog.Logs.Count;


            CurrentRank5Up = CurrentRank5 = CurrentRank4Up = CurrentRank4 = CurrentRank3 = 0;
            foreach (var grp in currentlog.Logs.GroupBy(x => x.Rank))
            {
                var rank = grp.First().Rank;
                if (rank == 5)
                {
                    var r5 = grp.ToList();
                    CurrentRank5 = r5.Count;
                    int upc = 0;
                    foreach (var item in r5)
                    {
                        if (current.Rank5Up.Contains(item.Name)) upc++;
                    }
                    CurrentRank5Up = upc;
                }
                if (rank == 4)
                {
                    var r4 = grp.ToList();
                    CurrentRank4 = r4.Count;
                    int upc = 0;
                    foreach (var item in r4)
                    {
                        if (current.Rank4Up.Contains(item.Name)) upc++;
                    }
                    CurrentRank4Up = upc;
                }
                if (rank == 3)
                {
                    var r3 = grp.ToList();
                    CurrentRank3 = r3.Count;
                }
            }



            CurrentRoundIndex = currentlog.GreaterRounds.Count;

        label_current_round:

            var curround = DDCL.CurrentUser.GetLastGreaterRoundByCategorizedType(PoolType);
            if (curround == null) curround = new DDCLRoundLogItem { Logs = new List<DDCLGachaLogItem>() };

            int prevr5p = 0, prev = 0, curr5p = 0, cur = 0;

            foreach (var item in curround.Logs)
            {
                if (item.BannerInternalID != currentlog?.BannerInternalID)
                {
                    prev++;
                    if (item.Rank == 5)
                    {
                        prevr5p = prev;
                        prev = 0;
                    }
                }
                else
                {
                    cur++;
                    if (item.Rank == 5)
                    {
                        curr5p = cur;
                        cur = 0;
                    }
                }
            }
            SetCurrentRoundValues(prevr5p, prev, curr5p, cur);

        }

        public virtual void RefreshGlobalDashboard()
        {

        }

        public void RebuildRank5List(List<DDCLGachaLogItem> logs)
        {
            var rounds = logs.GroupBy(x => x.RoundID);
            var ls = new List<DDCVUnitIndicatorModel>();
            var upls = new List<DDCVUnitIndicatorModel>();
            var tz = DDCL.CurrentUser.GetActivatingTimeZone();
            int idx = 1;
            int upc = 0;
            foreach (var rnd in rounds)
            {
                var rndl = rnd.ToList();
                if (rndl.Count > 0)
                {
                    var last = rndl.Last();
                    if (last.Rank < 5) continue;
                    var item = new DDCVUnitIndicatorModel
                    {
                        Name = last.Name,
                        Time = DDCL.GetTimeOffset(last.Time, tz).ToLocalTime(),
                        Version = DDCL.BannerLib.GetVersion(last.VersionID).Version,
                        Banner = DDCL.BannerLib.GetBanner(last.BannerInternalID).Name,
                        Count = rndl.Count,
                        Index = idx,
                        BrMax = BasicRoundTotal,
                    };
                    item.BrLucky = brLuckyLimit > item.Count;
                    item.BrUnlucky = item.Count > softPityThreshold;

                    ls.Add(item);

                    var banner = DDCL.BannerLib.GetBanner(last.BannerInternalID);
                    if(banner.Rank5Up.Count > 0)
                    {
                        upc += item.Count;
                        if (banner.Rank5Up.Contains(item.Name))
                        {
                            item.RndCount = upc;
                            item.RndLucky = rndLuckyLimit > item.RndCount;
                            item.RndUnlucky = item.RndCount > rndUnluckyLimit;
                            item.RndMax = CurrentRoundTotal;
                            upc = 0;
                            idx++;
                            upls.Add(item);
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
            }
            ls.Reverse();
            upls.Reverse();
            Rank5List = new ObservableCollection<DDCVUnitIndicatorModel>(ls);
            Rank5UpList = new ObservableCollection<DDCVUnitIndicatorModel>(upls);
        }



    }
}
