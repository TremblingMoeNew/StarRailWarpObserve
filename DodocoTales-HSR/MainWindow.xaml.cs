using DodocoTales.SR.Common;
using DodocoTales.SR.Gui;
using DodocoTales.SR.Gui.Models;
using DodocoTales.SR.Gui.Views;
using DodocoTales.SR.Gui.Views.Windows;
using DodocoTales.SR.Library;
using DodocoTales.SR.Loader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace DodocoTales
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DDCV.MainWindow = this;
            DDCV.MainNavigater = MainNavigator;
        //    DDCV.RegisterMainScreens();
            DDCS.CurUserSwapCompleted += OnUIDSwapCompleted;
            DDCS.ProxyCaptured += OnProxyCaptured;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
        //    DDCLog.InitHint();
        //    DDCG.Initialize();
            await DDCL.BannerLib.LoadLibraryAsync();
       //     await DDCL.UnitLib.LoadLibraryAsync();
            await DDCL.UserDataLib.LoadLocalGachaLogsAsync();
            await DDCL.GameClientLib.LoadLibraryAsync();
       //     DDCL.CurrentUser.SwapUser(0);
       //     DDCV.RefreshAll();

            //HomeScn.Refresh();
            //BanViewScn.SetBanner(201, 201101);
            //BanViewScn.SetBanner(202, 202102);
            //BanViewScn.Refresh();
            //Card.Refresh();
            // Card2.Refresh();
            ///Card3.Refresh();
            //DDCLog.Info(DCLN.Debug, JsonConvert.SerializeObject(DDCL.CurrentUser.GreaterRounds,Formatting.Indented));
        }


        private void MainPanel_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
       //     var item = MainPanel.SelectedItem as DDCVMainPanelItemModel;
        //    if (item != null)
        //        DDCV.SwapMainScreen(item.Tag);
        }

        private void TreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
         //   var item = (sender as Label).DataContext as DDCVMainPanelItemModel;
         //   if (item == MainPanel.SelectedItem)
        //        DDCV.SwapMainScreen(item.Tag);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var selected = DDCL.GameClientLib.GetSelectedClient();
            if (selected == null)
                if (!DDCV.ShowWindowDialog(new DDCVGameClientManagerWindow()))
                    return;

            selected = DDCL.GameClientLib.GetSelectedClient();
            if (selected != null)
                Console.WriteLine(DDCG.GameClientLoader.GetAuthkeyFromWebCache(selected));
        }

        private void UpdatePanelButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePanel.IsOpen = true;
        }

        private void OnUIDSwapCompleted(long uid)
        {
            VM.RefreshCurrentUID();
       //     if (!VM.IsInUpdate) DDCV.RefreshAll();
        }

        private async void OnProxyCaptured()
        {
            Action action = async () => {
                if (VM.IsInProxyUpdateAppended)
                {
                    await VM.WishLogUpdateAppendedFromProxy();
                }
                else if (VM.IsInProxyUpdateFull)
                {
                    await VM.WishLogUpdateFullFromProxy();
                }
                else
                {
                    VM.IsProxyModeOn = false;
                }
            };
            await Dispatcher.BeginInvoke(action);
        }

        private async void UpdateWishButton_Click(object sender, RoutedEventArgs e)
        {
            await VM.WishLogUpdateAppended();
        }
        private async void UpdateWishFullModeButton_Click(object sender, RoutedEventArgs e)
        {
            await VM.WishLogUpdateFull();
        }

        private void OpenGameClientManagerWindowButton_Click(object sender, RoutedEventArgs e)
        {
            DDCV.ShowWindowDialog(new DDCVGameClientManagerWindow());
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // Proxy Ensure Stopped
            DDCG.ProxyLoader.EndProxy();
            Application.Current.Shutdown();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
        //    DDCV.ShowWindowDialog(new DDCVSettingsWindow());
        }
    }
}
