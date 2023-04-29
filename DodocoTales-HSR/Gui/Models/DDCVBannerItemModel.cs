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
    public class DDCVBannerItemModel : ObservableObject
    {
        private ulong versionId;
        public ulong VersionID
        {
            get => versionId;
            set => SetProperty(ref versionId, value);
        }
        private ulong bannerInternalId;
        public ulong BannerInternalID
        {
            get => bannerInternalId;
            set => SetProperty(ref bannerInternalId, value);
        }
        private string version;
        public string Version
        {
            get => version;
            set => SetProperty(ref version, value);
        }
        private string bannerName;
        public string BannerName
        {
            get => bannerName;
            set => SetProperty(ref bannerName, value);
        }
        private string bannerHint;
        public string BannerHint
        {
            get => bannerHint;
            set => SetProperty(ref bannerHint, value);
        }
        private DDCCPoolType poolType;
        public DDCCPoolType PoolType
        {
            get => poolType;
            set => SetProperty(ref poolType, value);
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

        private int total;
        public int Total
        {
            get => total;
            set => SetProperty(ref total, value);
        }
        private int rank5;
        public int Rank5
        {
            get => rank5;
            set => SetProperty(ref rank5, value);
        }
        private int rank4;
        public int Rank4
        {
            get => rank4;
            set => SetProperty(ref rank4, value);
        }
        private ObservableCollection<DDCVUnitIndicatorModel> rank5Ups;
        public ObservableCollection<DDCVUnitIndicatorModel> Rank5Ups
        {
            get => rank5Ups;
            set => SetProperty(ref rank5Ups, value);
        }
        public bool Rank5UpsAvailAble
        {
            get => Rank5Ups != null && Rank5Ups.Count > 0;
        }
        private ObservableCollection<DDCVUnitIndicatorModel> rank4Ups;
        public ObservableCollection<DDCVUnitIndicatorModel> Rank4Ups
        {
            get => rank4Ups;
            set => SetProperty(ref rank4Ups, value);
        }
        public bool Rank4UpsAvailAble
        {
            get => Rank4Ups != null && Rank4Ups.Count > 0;
        }
    }
}
