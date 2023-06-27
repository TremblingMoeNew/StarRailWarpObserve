using DodocoTales.SR.Gui.Models;
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
    /// DDCVSettingsWindowUsersScreen.xaml 的交互逻辑
    /// </summary>
    public partial class DDCVSettingsWindowUsersScreen : DDCVSwapableScreen
    {
        public DDCVSettingsWindowUsersScreen()
        {
            InitializeComponent();
        }
        public override void Refresh()
        {
            VM.Refresh();
        }

        private void SwapButton_Click(object sender, RoutedEventArgs e)
        {
            VM.SwapUser((sender as Button).DataContext as DDCVUserlogItem);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            VM.RemoveUser((sender as Button).DataContext as DDCVUserlogItem);
        }
    }
}
