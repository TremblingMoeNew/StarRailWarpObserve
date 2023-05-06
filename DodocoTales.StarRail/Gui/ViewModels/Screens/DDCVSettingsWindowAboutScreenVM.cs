using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.ViewModels.Screens
{
    public class DDCVSettingsWindowAboutScreenVM : ObservableObject
    {
        private string clientVersion;
        public string ClientVersion
        {
            get => clientVersion;
            set => SetProperty(ref clientVersion, value);
        }

        private string bannerLibraryVersion;
        public string BannerLibraryVersion
        {
            get => bannerLibraryVersion;
            set => SetProperty(ref bannerLibraryVersion, value);
        }

        public DDCVSettingsWindowAboutScreenVM()
        {
            ClientVersion = DDCL.MetaVersionLib.ClientVersion;
            BannerLibraryVersion = DDCL.MetaVersionLib.BannerLibraryVersion;
        }
    }
}
