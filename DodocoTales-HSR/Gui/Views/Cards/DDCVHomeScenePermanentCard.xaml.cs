using Panuon.UI.Silver;
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

namespace DodocoTales.SR.Gui.Views.Cards
{
    /// <summary>
    /// DDCVHomeScenePermanentCard.xaml 的交互逻辑
    /// </summary>
    public partial class DDCVHomeScenePermanentCard : UserControl
    {
        public DDCVHomeScenePermanentCard()
        {
            InitializeComponent();
        }
        public void Refresh()
        {
            VM.ReloadData();
        }

        private void InfoPanelStatusIndicator_Checked(object sender, RoutedEventArgs e)
        {
            if (InfoPanelStatusIndicator.IsChecked == true)
            {
                AnimationHelper.SetFadeOut(Dashboard, true);
                Dashboard.Visibility = Visibility.Collapsed;
                Details.Visibility = Visibility.Visible;
                AnimationHelper.SetFadeIn(Details, true);
            }
            else
            {
                AnimationHelper.SetFadeOut(Details, true);
                Details.Visibility = Visibility.Collapsed;
                Dashboard.Visibility = Visibility.Visible;
                AnimationHelper.SetFadeIn(Dashboard, true);
            }
        }
    }
}
