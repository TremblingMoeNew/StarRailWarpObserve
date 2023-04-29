using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.Models
{
    public class DDCVVersionItemModel : ObservableObject
    {
        private ulong versionId;
        public ulong VersionID
        {
            get => versionId;
            set => SetProperty(ref versionId, value);
        }

        private string versionName;
        public string VersionName
        {
            get => versionName;
            set => SetProperty(ref versionName, value);
        }
        private string version;
        public string Version
        {
            get => version;
            set => SetProperty(ref version, value);
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

        private ObservableCollection<DDCVBannerItemModel> banners;
        public ObservableCollection<DDCVBannerItemModel> Banners
        {
            get => banners;
            set => SetProperty(ref banners, value);
        }

        public int BannersCnt
        {
            get => Banners.Count;
        }
        public int CharacterEventCnt
        {
            get => Banners.Where(x => x.PoolType == DDCCPoolType.CharacterEvent).Sum(x => x.Total);
        }
        public int LCEventCnt
        {
            get => Banners.Where(x => x.PoolType == DDCCPoolType.LCEvent).Sum(x => x.Total);
        }
        public int PermanentCnt
        {
            get => Banners.Where(x => x.PoolType == DDCCPoolType.Permanent).Sum(x => x.Total);
        }
        public int Total
        {
            get => Banners.Sum(x => x.Total);
        }
        public int Rank5
        {
            get => Banners.Sum(x => x.Rank5);
        }
        public int Rank4
        {
            get => Banners.Sum(x => x.Rank4);
        }
        private ObservableCollection<DDCVUnitIndicatorModel> rank5s;
        public ObservableCollection<DDCVUnitIndicatorModel> Rank5s
        {
            get => rank5s;
            set => SetProperty(ref rank5s, value);
        }
    }
}
