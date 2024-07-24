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

namespace DodocoTales.SR.Gui.Views.Dialogs
{
    /// <summary>
    /// DDCVImportDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DDCVImportDialog : Window
    {
        public DDCVImportDialog()
        {
            InitializeComponent();
        }

        public async void Import()
        {
            if (await VM.LoadLog())
                DDCV.ShowWindowDialog(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            if (VM.IsImportReady())
            {
                if (await VM.Import())
                {
                    Close();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            if (VM.Skip())
            {
                Close();
            }
        }
    }
}
