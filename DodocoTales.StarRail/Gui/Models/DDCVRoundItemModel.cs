using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Library.CurrentUser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.Models
{
    public class DDCVRoundItemModel : ObservableObject
    {
        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

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

        private int index;
        public int Index
        {
            get => index;
            set => SetProperty(ref index, value);
        }

        private DDCLRoundLogItem logItem;
        public DDCLRoundLogItem LogItem
        {
            get => logItem;
            set => SetProperty(ref logItem, value);
        }

        public int Count
        {
            get => LogItem?.Logs.Count ?? 0;
        }

        public int CountCurrent
        {
            get => LogItem?.Logs.Count(x => x.BannerInternalID == BannerInternalID) ?? 0;
        }

        public int CountInherited
        {
            get => Count - CountCurrent;
        }

        public int Rank5
        {
            get => LogItem?.Logs.Count(x => x.Rank == 5) ?? 0;
        }

        public int Rank4
        {
            get => LogItem?.Logs.Count(x => x.Rank == 4) ?? 0;
        }

    }
}
