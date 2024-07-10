using Newtonsoft.Json;
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
    /// DDCVExportDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DDCVExportDialog : Window
    {
        public DDCVExportDialog()
        {
            InitializeComponent();
        }

        private void FilePathBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VM.SelectExportPath();
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            VM.SelectedUIDMulti.Clear();
            foreach(KeyValuePair<long, bool> x in MultiUIDSelect.SelectedItems)
            {
                VM.SelectedUIDMulti.Add(x.Key);
            }
            if(await VM.Export())
                Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
