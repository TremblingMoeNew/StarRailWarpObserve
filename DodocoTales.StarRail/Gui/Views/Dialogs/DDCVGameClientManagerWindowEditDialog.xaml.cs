using DodocoTales.SR.Library.GameClient.Models;
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
    /// DDCVGameClientManagerWindowEditDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DDCVGameClientManagerWindowEditDialog : Window
    {
        public DDCVGameClientManagerWindowEditDialog()
        {
            InitializeComponent();
        }
        public void LoadGameClientItem(DDCLGameClientItem item)
        {
            VM.LoadGameClientItem(item);
        }
        public void NewGameClientItem()
        {
            VM.NewGameClientItem();
        }
        private void FilePathBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VM.EditGamePath();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            VM.Delete();
            Close();
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(await VM.Save()) Close();
        }
        private void SaveAsCopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (VM.SaveAsCopy()) Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

