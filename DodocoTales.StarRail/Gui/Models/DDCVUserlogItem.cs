using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.Models
{
    public class DDCVUserlogItem : ObservableObject
    {
        private long uid;
        public long UID
        {
            get => uid;
            set => SetProperty(ref uid, value);
        }

        private int timezone;
        public int TimeZone
        {
            get => timezone;
            set => SetProperty(ref timezone, value);
        }

        private DDCLGameClientType clientType;
        public DDCLGameClientType ClientType
        {
            get => clientType;
            set => SetProperty(ref clientType, value);
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }
    }
}
