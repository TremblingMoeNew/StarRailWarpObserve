using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DodocoTales.SR.Gui.Views.Screens
{
    /// <summary>
    /// DDCVBannerViewScreen.xaml 的交互逻辑
    /// </summary>
    public partial class DDCVBannerViewScreen : DDCVSwapableScreen
    {
        public DDCVBannerViewScreen()
        {
            InitializeComponent();
        }
        public void SetBanner(ulong versionid, ulong bannerinternalid)
        {
            VM.SetBanner(versionid, bannerinternalid);
        }
        public override void Refresh()
        {
            VM.ReloadData();
        }
    }
}
