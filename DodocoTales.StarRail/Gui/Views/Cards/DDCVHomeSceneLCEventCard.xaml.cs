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
    /// DDCVHomeSceneLCEventCard.xaml 的交互逻辑
    /// </summary>
    public partial class DDCVHomeSceneLCEventCard : UserControl
    {
        public DDCVHomeSceneLCEventCard()
        {
            InitializeComponent();
        }


        public void Refresh()
        {
            VM.ReloadData();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var btn = sender as RadioButton;
            if (btn != null)
            {
                var idx = Convert.ToInt32(btn.Tag);
                VM.UnitIndicatorCurrentPageIndex = idx;
            }
        }
    }
}
