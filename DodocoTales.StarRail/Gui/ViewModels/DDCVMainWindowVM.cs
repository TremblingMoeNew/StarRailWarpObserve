using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Common;
using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Gui.Enums;
using DodocoTales.SR.Gui.Models;
using DodocoTales.SR.Gui.Views.Windows;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Loader;
using DodocoTales.SR.Loader.Models;
using Newtonsoft.Json;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.ViewModels
{
    public class DDCVMainWindowVM : ObservableObject
    {
        private ObservableCollection<DDCVMainPanelItemModel> menuItems;
        public ObservableCollection<DDCVMainPanelItemModel> MenuItems
        {
            get => menuItems;
            set => SetProperty(ref menuItems, value);
        }

        private bool isProxyMode;
        public bool IsProxyMode
        {
            get => isProxyMode;
            set
            {
                if (value)
                {
                    if (DDCG.ProxyLoader.InitializeProxy())
                    {
                        Notice.Show("代理服务器初始化完毕", "代理模式", MessageBoxIcon.Info);
                    }
                }
                else
                {
                    IsProxyModeOn = false;
                }
                SetProperty(ref isProxyMode, value);
            }
        }

        private bool isProxyModeOn;
        public bool IsProxyModeOn
        {
            get => isProxyModeOn;
            set
            {
                if (value)
                {
                    if (!isProxyModeOn)
                    {
                        IsWaiting = value;
                        DDCG.ProxyLoader.StartProxy();
                        Notice.Show("代理模式已启动，请在游戏中重新打开跃迁历史记录页面", "代理模式", MessageBoxIcon.Info);
                        OnProxyModeOn();
                    }
                }
                else
                {
                    DDCG.ProxyLoader.EndProxy();
                    IsWaiting = false;
                    IsInProxyUpdateAppended = false;
                    IsInProxyUpdateFull = false;
                    if (isProxyModeOn)
                    {
                        Notice.Show("代理模式已终止", "代理模式", MessageBoxIcon.Info);
                        OnProxyModeOff();
                    }
                }
                SetProperty(ref isProxyModeOn, value);
            }
        }

        private bool isInUpdate;
        public bool IsInUpdate
        {
            get => isInUpdate;
            set
            {
                SetProperty(ref isInUpdate, value);
                IsWaiting = value;
            }
        }
        private bool isWaiting;
        public bool IsWaiting
        {
            get => isWaiting;
            set => SetProperty(ref isWaiting, value);
        }

        private long currentUID;
        public long CurrentUID
        {
            get => currentUID;
            set => SetProperty(ref currentUID, value);
        }
        
        private int currentUserTimeZone;
        public int CurrentUserTimeZone
        {
            get => currentUserTimeZone;
            set => SetProperty(ref currentUserTimeZone, value);
        }
        private DDCLGameClientType currentUserClientType;
        public DDCLGameClientType CurrentUserClientType
        {
            get => currentUserClientType;
            set => SetProperty(ref currentUserClientType, value);
        }
        
        public bool IsInProxyUpdateAppended { get; set; }
        public bool IsInProxyUpdateFull { get; set; }

        public DDCVMainWindowVM()
        {
            MenuItems = new ObservableCollection<DDCVMainPanelItemModel>()
            {
                new DDCVMainPanelItemModel("首页","Home", 0) { IsSelected = true },
                new DDCVMainPanelItemModel("统计","Version",0)
                {
                    /*
                    MenuItems=new ObservableCollection<DDCVMainPanelItemModel>
                    {
                        new DDCVMainPanelItemModel("按照版本","Version", 1),
                        new DDCVMainPanelItemModel("按照类型","PoolType",1),
                    }
                    */
                },
                //new DDCVMainPanelItemModel("预测", "Prediction", 0),
            };
            CurrentUID = -1;
            HintBannerInitialize();
        }

        public void RefreshCurrentUID()
        {
            var user = DDCL.CurrentUser.OriginalLogs;
            if (user == null) CurrentUID = -1;
            else
            {
                CurrentUID = user.UID;
                CurrentUserTimeZone = user.TimeZone;
                CurrentUserClientType = user.ClientType;
            }
            DDCV.RefreshAll();
            
        }

        public async Task WishLogUpdateAppended()
        {
            
            if (IsProxyMode)
            {
                if (DDCG.ProxyLoader.Authkey == null)
                {
                    IsInProxyUpdateAppended = true;
                    IsProxyModeOn = true;
                }
                else
                {
                    await WishLogUpdateAppendedFromProxy();
                }
            }
            else
                await WishLogUpdateAppendedFromCache();
            
        }

        public async Task WishLogUpdateFull()
        {
            
            if (IsProxyMode)
            {
                if (DDCG.ProxyLoader.Authkey == null)
                {
                    IsInProxyUpdateFull = true;
                    IsProxyModeOn = true;
                }
                else
                {
                    WishLogUpdateFullFromProxy();
                }
            }
            else
                await WishLogUpdateFullFromCache();
            
        }
        public async Task WishLogUpdateAppendedFromCache()
        {
            
            var client = DDCL.GameClientLib.GetSelectedClient();
            if (client == null)
                if (!DDCV.ShowWindowDialog(new DDCVGameClientManagerWindow()))
                    return;

            client = DDCL.GameClientLib.GetSelectedClient();
            var authkey = DDCG.GameClientLoader.GetAuthkeyFromWebCache(client);

            if (authkey == null)
            {
                Notice.Show("更新失败，未能找到跃迁记录网址。\n请确认您的崩坏：星穹铁道客户端地址设置是否正确，且是否在游戏中正确打开跃迁历史记录。", "跃迁记录更新失败", MessageBoxIcon.Error);
                //DDCLog.Info(DCLN.Gui, "Wish log update failed: Authkey not found. (Append mode, Cache mode)");
                OnFetchGachaLogFailed();
                return;
            }
            IsInUpdate = true;
            var uid = await DDCG.WebLogLoader.TryConnectAndGetUid(authkey, client.ClientType);
            if (uid < 0)
            {
                Notice.Show("更新失败，未能获取到跃迁记录\n请确认网络是否连接正常，设置的客户端类型是否正确，近六个月内是否进行过跃迁。\n请在游戏中重新打开跃迁历史记录页面。若仍然失败，可尝试在客户端管理中清除缓存。", "跃迁记录更新失败", MessageBoxIcon.Error);
                IsInUpdate = false;
                OnFetchGachaLogFailed();
                //DDCLog.Info(DCLN.Gui, "Wish log update failed: Fetch failed. (Append mode, Cache mode)");
                return;
            }
            var user = DDCL.UserDataLib.GetUserLogByUid(uid);
            if (user.ClientType == DDCLGameClientType.Unknown)
            {
                user.ClientType = client.ClientType;
            }
            DDCL.CurrentUser.SwapUser(user);
            
            await DDCG.WebLogLoader.GetGachaLogsAsNormalMode(authkey, client.ClientType);
            IsInUpdate = false;
            Notice.Show("跃迁记录常规更新完毕", "跃迁记录更新完毕", MessageBoxIcon.Success);
            OnFetchGachaLogSucceed();
            DDCS.Emit_CurUserUpdateCompleted();
            //DDCLog.Info(DCLN.Gui, "Wish log update completed. (Append mode, Cache mode)");
            
        }
        public async Task WishLogUpdateFullFromCache()
        {
            
            var client = DDCL.GameClientLib.GetSelectedClient();
            if (client == null)
                if (!DDCV.ShowWindowDialog(new DDCVGameClientManagerWindow()))
                    return;

            client = DDCL.GameClientLib.GetSelectedClient();
            var authkey = DDCG.GameClientLoader.GetAuthkeyFromWebCache(client);

            if (authkey == null)
            {
                Notice.Show("更新失败，未能找到跃迁记录网址。\n请确认您的崩坏：星穹铁道客户端地址设置是否正确，且是否在游戏中正确打开跃迁历史记录。", "跃迁记录更新失败", MessageBoxIcon.Error);
                //DDCLog.Info(DCLN.Gui, "Wish log update failed: Authkey not found. (Full mode, Cache mode)");
                OnFetchGachaLogFailed(); 
                return;
            }
            IsInUpdate = true;
            var uid = await DDCG.WebLogLoader.TryConnectAndGetUid(authkey, client.ClientType);
            if (uid < 0)
            {
                Notice.Show("更新失败，未能获取到跃迁记录\n请确认网络是否连接正常，设置的客户端类型是否正确，近六个月内是否进行过跃迁。\n请在游戏中重新打开跃迁历史记录页面。若仍然失败，可尝试在客户端管理中清除缓存。", "跃迁记录更新失败", MessageBoxIcon.Error);
                //DDCLog.Info(DCLN.Gui, "Wish log update failed: Fetch failed. (Full mode, Cache mode)");
                IsInUpdate = false;
                OnFetchGachaLogFailed(); 
                return;
            }
            
            var user = DDCL.UserDataLib.GetUserLogByUid(uid);
            if (user.ClientType == DDCLGameClientType.Unknown)
            {
                user.ClientType = client.ClientType;
            }
            DDCL.CurrentUser.SwapUser(user);
            
            await DDCG.WebLogLoader.GetGachaLogsAsFullMode(authkey, client.ClientType);
            IsInUpdate = false;
            Notice.Show("跃迁记录全量更新完毕", "跃迁记录更新完毕", MessageBoxIcon.Success);
            OnFetchGachaLogSucceed();
            DDCS.Emit_CurUserUpdateCompleted();
            //DDCLog.Info(DCLN.Gui, "Wish log update completed. (Full mode, Cache mode)");

        }

        public async Task WishLogUpdateAppendedFromProxy()
        {
            
            if (IsProxyModeOn) IsProxyModeOn = false;
            var authkey = DDCG.ProxyLoader.Authkey;
            if (authkey == null)
            {
                Notice.Show("更新失败，未能找到跃迁记录网址。\n请联系开发者。", "跃迁记录更新失败", MessageBoxIcon.Error);
                //DDCLog.Info(DCLN.Gui, "Wish log update failed: Authkey not found. (Append mode, Proxy mode)");
                OnFetchGachaLogFailed();
                return;
            }
            var clientType = DDCG.ProxyLoader.CapturedClientType;
            IsInUpdate = true;
            
            var uid = await DDCG.WebLogLoader.TryConnectAndGetUid(authkey, clientType);
            if (uid < 0)
            {
                Notice.Show("更新失败，未能获取到跃迁记录\n请确认网络是否连接正常，近六个月内是否进行过跃迁。\n请重新启动代理模式，并在游戏中重新打开跃迁历史记录页面。", "跃迁记录更新失败", MessageBoxIcon.Error);
                IsInUpdate = false;
                //DDCLog.Info(DCLN.Gui, "Wish log update failed: Fetch failed. (Append mode, Proxy mode)");
                DDCG.ProxyLoader.Authkey = null;
                OnFetchGachaLogFailed();
                return;
            }
            var user = DDCL.UserDataLib.GetUserLogByUid(uid);
            if (user.ClientType == DDCLGameClientType.Unknown)
            {
                user.ClientType = clientType;
            }
            DDCL.CurrentUser.SwapUser(user);
            await DDCG.WebLogLoader.GetGachaLogsAsNormalMode(authkey, clientType);
            IsInUpdate = false;
            Notice.Show("跃迁记录常规更新完毕", "跃迁记录更新完毕", MessageBoxIcon.Success);
            OnFetchGachaLogSucceed();
            DDCS.Emit_CurUserUpdateCompleted();
            //DDCLog.Info(DCLN.Gui, "Wish log update completed. (Append mode, Proxy mode)");

        }

        public async Task WishLogUpdateFullFromProxy()
        {
            
            if (IsProxyModeOn) IsProxyModeOn = false;
            var authkey = DDCG.ProxyLoader.Authkey;
            if (authkey == null)
            {
                Notice.Show("更新失败，未能找到跃迁记录网址。\n请联系开发者。", "跃迁记录更新失败", MessageBoxIcon.Error);
                OnFetchGachaLogFailed();
                //DDCLog.Info(DCLN.Gui, "Wish log update failed: Authkey not found. (Full mode, Proxy mode)");
                return;
            }
            var clientType = DDCG.ProxyLoader.CapturedClientType;
            IsInUpdate = true;
            var uid = await DDCG.WebLogLoader.TryConnectAndGetUid(authkey, clientType);
            if (uid < 0)
            {
                Notice.Show("更新失败，未能获取到跃迁记录\n请确认网络是否连接正常，近六个月内是否进行过跃迁。\n请重新启动代理模式，并在游戏中重新打开跃迁历史记录页面。", "跃迁记录更新失败", MessageBoxIcon.Error);
                IsInUpdate = false;
                //DDCLog.Info(DCLN.Gui, "Wish log update failed: Fetch failed. (Full mode, Proxy mode)");
                DDCG.ProxyLoader.Authkey = null;
                OnFetchGachaLogFailed();
                return;
            }
            var user = DDCL.UserDataLib.GetUserLogByUid(uid);
            if (user.ClientType == DDCLGameClientType.Unknown)
            {
                user.ClientType = clientType;
            }
            DDCL.CurrentUser.SwapUser(user);
            await DDCG.WebLogLoader.GetGachaLogsAsFullMode(authkey, clientType);
            IsInUpdate = false;
            Notice.Show("跃迁记录全量更新完毕", "跃迁记录更新完毕", MessageBoxIcon.Success);
            OnFetchGachaLogSucceed();
            DDCS.Emit_CurUserUpdateCompleted();
            //DDCLog.Info(DCLN.Gui, "Wish log update completed. (Full mode, Proxy mode)");

        }


        #region HintBanner
        private DDCVHintBannerStatus hintBannerStatus;
        public DDCVHintBannerStatus HintBannerStatus
        {
            get => hintBannerStatus;
            set => SetProperty(ref hintBannerStatus, value);
        }
        private string hintBannerContentUpdating;
        public string HintBannerContentUpdating
        {
            get => hintBannerContentUpdating;
            set
            {
                if (value == null)
                {
                    if (HintBannerStatus == DDCVHintBannerStatus.Updating)
                    {
                        HintBannerStatus = DDCVHintBannerStatus.NoHint;
                    }
                }
                SetProperty(ref hintBannerContentUpdating, value);
                if (value != null)
                {
                    if (HintBannerStatus < DDCVHintBannerStatus.Updating)
                    {
                        HintBannerStatus = DDCVHintBannerStatus.Updating;
                    }
                }
            }
        }
        private string hintBannerContentFetchingGachaLog;
        public string HintBannerContentFetchingGachaLog
        {
            get => hintBannerContentFetchingGachaLog;
            set
            {
                if (value == null)
                {
                    if (HintBannerStatus == DDCVHintBannerStatus.FetchingGachaLog)
                    {
                        if (HintBannerContentUpdating != null)
                        {
                            HintBannerStatus = DDCVHintBannerStatus.Updating;
                        }
                        else
                        {
                            HintBannerStatus = DDCVHintBannerStatus.NoHint;
                        }
                    }
                }
                SetProperty(ref hintBannerContentFetchingGachaLog, value);
                if (value != null) 
                {
                    if (HintBannerStatus < DDCVHintBannerStatus.FetchingGachaLog)
                    {
                        HintBannerStatus = DDCVHintBannerStatus.FetchingGachaLog;
                    }
                }
            }
        }

        private string hintBannerContentError;
        public string HintBannerContentError
        {
            get => hintBannerContentError;
            set
            {
                if (value == null)
                {
                    if (HintBannerContentFetchingGachaLog != null)
                    {
                        HintBannerStatus = DDCVHintBannerStatus.FetchingGachaLog;
                    }
                    else if (HintBannerContentUpdating != null)
                    {
                        HintBannerStatus = DDCVHintBannerStatus.Updating;
                    }
                    else
                    {
                        HintBannerStatus = DDCVHintBannerStatus.NoHint;
                    }
                }
                SetProperty(ref hintBannerContentError, value);
                if (value != null)
                {
                    HintBannerStatus = DDCVHintBannerStatus.Error;
                }
            }
        }



        private void HintBannerInitialize()
        {
            HintBannerStatus = DDCVHintBannerStatus.NoHint;
            HintBannerContentUpdating = null;
            HintBannerContentFetchingGachaLog = null;
            HintBannerContentError = null;

            DDCS.ImportStatusFromWebRefreshed += OnFetchGachaLogStatusReport;
            DDCS.ClientUpdateDownloadStatusReport += OnUpdateStatusReported;

            DDCS.ClientUpdateDownloadFailed += OnUpdateFailed;
            DDCS.DependencyUpdateDownloadFailed += OnUpdateFailed;
            DDCS.ClientUpdateDownloadCompleted += OnUpdateSucceed;
            DDCS.DependencyUpdateDownloadCompleted += OnUpdateSucceed;



        }

        private void OnFetchGachaLogStatusReport(DDCCPoolType type, int current_page)
        {
            HintBannerContentFetchingGachaLog  = $"正在获取跃迁记录： {DDCL.GetPoolTypeName(type)} 第 {current_page} 页";
        }

        private void OnUpdateStatusReported(dynamic var)
        {
            DDCGDownloadTask task = var;
            HintBannerContentUpdating = $"正在更新： {task.LocalFileName}  {task.DownloadedSize / 1024.0:F2}KB/{task.FileSize / 1024.0:F2}KB {task.EstSpeed / 1024:F2}KB/s EST:{(task.FileSize - task.DownloadedSize) / task.EstSpeed:F2}s";
        }

        private void OnUpdateFailed()
        {
            HintBannerContentError = "更新失败";
            HintBannerContentUpdating = null;
            Action action = () => {
                Thread.Sleep(5000);
                HintBannerContentError = null;
            };
            action.BeginInvoke(null, null);
        }
        private void OnFetchGachaLogFailed()
        {
            HintBannerContentError = "跃迁记录获取失败";
            HintBannerContentUpdating = null;
            Action action = () => {
                Thread.Sleep(5000);
                HintBannerContentError = null;
            };
            action.BeginInvoke(null, null);
        }

        private void OnUpdateSucceed()
        {
            HintBannerContentUpdating = null;
        }
        private void OnFetchGachaLogSucceed()
        {
            HintBannerContentFetchingGachaLog = null;
        }

        private void OnProxyModeOn()
        {
            HintBannerContentFetchingGachaLog = "代理模式启用中，请在游戏中重新打开跃迁历史记录页面";
        }

        private void OnProxyModeOff()
        {
            HintBannerContentFetchingGachaLog = null;
        }
        #endregion

    }

}
