using DodocoTales.SR.Common;
using DodocoTales.SR.Gui;
using DodocoTales.SR.Gui.Models;
using DodocoTales.SR.Gui.Views;
using DodocoTales.SR.Gui.Views.Dialogs;
using DodocoTales.SR.Gui.Views.Screens;
using DodocoTales.SR.Gui.Views.Windows;
using DodocoTales.SR.Library;
using DodocoTales.SR.Loader;
using Newtonsoft.Json;
using Panuon.UI.Silver;
using SkiaSharp.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
        public bool InitializeCompleted = false;

        public MainWindow()
        {
            InitializeComponent();
            DDCV.MainWindow = this;
            DDCV.MainNavigater = MainNavigator;

            DDCS.CurUserSwapCompleted += OnUIDSwapCompleted;
            DDCS.CurUserUpdateCompleted += OnCurrentUserUpdateCompleted;
            DDCS.ProxyCaptured += OnProxyCaptured;
            DDCS.ClientNeedsUpdate += OnClientUpdateDownloadStart;
            DDCS.ClientUpdateDownloadCompleted += OnClientUpdateDownloadCompleted;
            DDCS.ClientUpdateDownloadFailed += OnClientUpdateDownloadFailed;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;


            var arch = PlatformConfiguration.Is64Bit
                    ? PlatformConfiguration.IsArm ? "arm64" : "x64"
                    : PlatformConfiguration.IsArm ? "arm" : "x86";

            if (!DDCG.UpdateLoader.DependencyExist(arch))
            {
                Notice.Show("当前平台的SkiaSharp原生依赖库缺失，正在自动下载", "依赖补全", MessageBoxIcon.Error);
                DDCV.RegisterMainScreen("DownloadDependcies", new DDCVDependenciesDownloadScreen());
                DDCS.DependencyUpdateDownloadCompleted += OnDependencyUpdateCompleted;
                DDCG.UpdateLoader.DownloadDependency(arch);
            }
            else
            {
                DDCV.RegisterMainScreens();
                Initialize();
            }  
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DDCG.ProxyLoader.EndProxy();
            DDCG.UpdateLoader.ApplyUpdate();
        }

        public async void Initialize()
        {
            VM.IsInUpdate = true;
            await DDCL.MetaVersionLib.Initialize();
            bool versionchecked = await DDCG.UpdateLoader.CheckVersion();

            bool bannerlibloaded = await DDCL.BannerLib.LoadLibraryAsync();

            await DDCL.ExportersLib.LoadLibraryAsync();

            await DDCL.UserDataLib.LoadLocalGachaLogsAsync();

            await DDCL.GameClientLib.LoadLibraryAsync();

            await DDCL.SettingsLib.LoadSettingsAsync();

            if (!versionchecked)
            {
                Notice.Show("元数据更新检查失败，请检查网络连接。抽卡记录更新功能将被禁用。", "错误", MessageBoxIcon.Error);
            }
            if (!bannerlibloaded)
            {
                Notice.Show("卡池信息载入失败。", "错误", MessageBoxIcon.Error);
            }
            if (bannerlibloaded && DDCL.UserDataLib.UserExists(DDCL.SettingsLib.LastUserUID))
            {
                DDCL.CurrentUser.SwapUser(DDCL.SettingsLib.LastUserUID);
            }
            DDCV.RefreshAll();
            InitializeCompleted = true;
            if (DDCL.MetaVersionLib.FirstRunAfterUpdate)
            {
                Notice.Show("应用更新完毕", "更新", MessageBoxIcon.Info);
                DDCV.ShowWindowDialog(new DDCVSettingsWindow().SwapToChangeLogScreen());
            }
            VM.IsInUpdate = !(versionchecked && bannerlibloaded);
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
        }

        private void OnCurrentUserUpdateCompleted()
        {
            VM.RefreshCurrentUID();
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

        private async void OnClientUpdateDownloadFailed()
        {
            Action action = async () => {
                Notice.Show("更新下载失败", "更新", MessageBoxIcon.Error);
            };
            await Dispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
        }

        private async void OnDependencyUpdateCompleted()
        {
            Action action = () => {
                DDCV.RegisterMainScreens();
                DDCV.SwapMainScreen("Home");
                Notice.Show("当前平台的SkiaSharp原生依赖库补全完毕，程序继续启动", "依赖补全", MessageBoxIcon.Success);
                Initialize();
            };
            await Dispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
        }

        private async void UpdateWishButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePanel.IsOpen = true;
            if (!VM.IsWaiting) await VM.WishLogUpdateAppended();
        }
        private async void UpdateWishFullModeButton_Click(object sender, RoutedEventArgs e)
        {
            await VM.WishLogUpdateFull();
        }

        private void OpenGameClientManagerWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (!InitializeCompleted) return;
            DDCV.ShowWindowDialog(new DDCVGameClientManagerWindow());
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // Proxy Ensure Stopped

            Application.Current.Shutdown();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!InitializeCompleted) return;
            DDCV.ShowWindowDialog(new DDCVSettingsWindow());
        }

        private async void ExportLog_Click(object sender, RoutedEventArgs e)
        {
            if (!InitializeCompleted) return;
            DDCV.ShowWindowDialog(new DDCVExportDialog());
        }

        private async void ImportLog_Click(object sender, RoutedEventArgs e)
        {
            if (!InitializeCompleted) return;
            new DDCVImportDialog().Import();
        }

        private void UIDButton_Click(object sender, RoutedEventArgs e)
        {
            DDCV.ShowWindowDialog(new DDCVSettingsWindow().SwapToUsersScreen());
        }
    }
}
