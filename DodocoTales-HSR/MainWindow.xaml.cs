using DodocoTales.SR.Common;
using DodocoTales.SR.Gui;
using DodocoTales.SR.Gui.Models;
using DodocoTales.SR.Gui.Views;
using DodocoTales.SR.Gui.Views.Windows;
using DodocoTales.SR.Library;
using DodocoTales.SR.Loader;
using Newtonsoft.Json;
using Panuon.UI.Silver;
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
using System.Windows.Threading;

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
            DDCV.RegisterMainScreens();
            DDCS.CurUserSwapCompleted += OnUIDSwapCompleted;
            DDCS.ProxyCaptured += OnProxyCaptured;
            DDCS.ClientNeedsUpdate += OnClientUpdateDownloadStart;
            DDCS.ClientUpdateDownloadCompleted += OnClientUpdateDownloadCompleted;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DDCG.ProxyLoader.EndProxy();
            DDCG.UpdateLoader.ApplyUpdate();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //    DDCLog.InitHint();
            //await DDCG.UpdateLoader.UpdateBannerLibrary();

            await DDCL.MetaVersionLib.Initialize();
            await DDCG.UpdateLoader.CheckVersion();

            await DDCL.BannerLib.LoadLibraryAsync();
            await DDCL.UserDataLib.LoadLocalGachaLogsAsync();

            await DDCL.GameClientLib.LoadLibraryAsync();
            DDCL.CurrentUser.SwapUser(0);
            DDCV.RefreshAll();

            //Console.WriteLine(JsonConvert.SerializeObject(DDCL.CurrentUser.GreaterRounds, Formatting.Indented));
        }


        private void MainPanel_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = MainPanel.SelectedItem as DDCVMainPanelItemModel;
            if (item != null)
                DDCV.SwapMainScreen(item.Tag);
        }

        private void TreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as Label).DataContext as DDCVMainPanelItemModel;
            if (item == MainPanel.SelectedItem)
                DDCV.SwapMainScreen(item.Tag);
        }


        private void UpdatePanelButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePanel.IsOpen = true;
        }

        private void OnUIDSwapCompleted(long uid)
        {
            VM.RefreshCurrentUID();
            if (!VM.IsInUpdate) DDCV.RefreshAll();
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

        private async void OnClientUpdateDownloadStart()
        {
            Action action = async () => {
                Notice.Show("正在下载更新……", "更新", MessageBoxIcon.Info);
            };
            await Dispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
            DDCG.UpdateLoader.DownloadClient();
        }

        private async void OnClientUpdateDownloadCompleted()
        {
            Action action = async () => {
                Notice.Show("更新下载完毕，将于重启时生效", "更新", MessageBoxIcon.Info);
            };
            await Dispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
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

            Application.Current.Shutdown();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
        //    DDCV.ShowWindowDialog(new DDCVSettingsWindow());
        }
    }
}
