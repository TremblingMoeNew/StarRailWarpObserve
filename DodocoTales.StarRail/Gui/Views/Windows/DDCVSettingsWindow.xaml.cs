using DodocoTales.SR.Gui.Models;
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
using System.Windows.Shapes;

namespace DodocoTales.SR.Gui.Views.Windows
{
    /// <summary>
    /// DDCVSettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DDCVSettingsWindow : WindowX
    {
        public DDCVSettingsWindow()
        {
            InitializeComponent();
            VM.Initialize(Navigator);
        }

        private void MainPanel_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = MainPanel.SelectedItem as DDCVMainPanelItemModel;
            if (item != null)
                VM.SwapScreen(item.Tag);
        }
        private void TreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as Label).DataContext as DDCVMainPanelItemModel;
            if (item == MainPanel.SelectedItem)
                VM.SwapScreen(item.Tag);
        }

        public DDCVSettingsWindow SwapToUsersScreen()
        {
            VM.SelectMenu("Users");
            return this;
        }

        public DDCVSettingsWindow SwapToChangeLogScreen()
        {
            VM.SelectMenu("ChangeLog");
            return this;
        }
    }
}
