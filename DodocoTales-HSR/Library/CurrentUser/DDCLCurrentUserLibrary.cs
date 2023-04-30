using DodocoTales.SR.Common;
using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Library.CurrentUser.Models;
using DodocoTales.SR.Library.UserDataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.CurrentUser
{
    public class DDCLCurrentUserLibrary
    {
        public DDCLUserGachaLog OriginalLogs { get; set; }

        public SortedList<ulong, DDCLGachaLogItem> Logs { get; set; }
        
        public List<DDCLRoundLogItem> BasicRounds { get; set; }
        public List<DDCLRoundLogItem> GreaterRounds { get; set; }
        public List<DDCLBannerLogItem> Banners { get; set; }
        

        public DDCLCurrentUserLibrary()
        {
            Logs = new SortedList<ulong, DDCLGachaLogItem>();
            
            BasicRounds = new List<DDCLRoundLogItem>();
            GreaterRounds = new List<DDCLRoundLogItem>();
            Banners = new List<DDCLBannerLogItem>();
            
        }

        
        // 查找
        // Banners
        public DDCLBannerLogItem GetBanner(ulong bannerinternalid)
            => Banners.Find(x => x.BannerInternalID == bannerinternalid);
        public int GetBannerIndex(ulong bannerinternalid)
            => Banners.FindIndex(x => x.BannerInternalID == bannerinternalid);
        public List<DDCLBannerLogItem> GetBannersByVersion(ulong versionid)
            => Banners.FindAll(x => x.VersionID == versionid);
        public List<DDCLBannerLogItem> GetBannersByCategorizedType(DDCCPoolType type)
            => Banners.FindAll(x => x.CategorizedGachaType == type);
        public List<DDCLBannerLogItem> GetBannersByVersionAndType(ulong versionid, DDCCPoolType type)
            => Banners.FindAll(x => x.VersionID == versionid && x.CategorizedGachaType == type);
        // GreaterRounds
        // BasicRounds

        
        // Logs
        public DDCLGachaLogItem GetItem(ulong internalid)
            => Logs[internalid];
        public int GetItemIndex(ulong id)
            => Logs.IndexOfKey(id);
        public int GetLastRank4Distance(ulong id)
        {
            int idx = GetItemIndex(id);
            var logs_values = Logs.Values;
            DDCLGachaLogItem startitem = logs_values[idx];
            int dis = 0;
            for (int i = idx - 1; i >= 0; i--)
            {
                if (logs_values[i].PoolType == startitem.PoolType)
                {
                    dis++;
                    if (startitem.Rank == 4) break;
                }
            }
            return dis;
        }
        public int GetLastRank4Distance(ulong id, DDCCUnitType type)
        {
            int idx = GetItemIndex(id);
            var logs_values = Logs.Values;
            DDCLGachaLogItem startitem = logs_values[idx];
            int dis = 0;
            for (int i = idx - 1; i >= 0; i--)
            {
                var currentitem = logs_values[i];
                if (currentitem.PoolType == startitem.PoolType)
                {
                    dis++;
                    if (currentitem.Rank == 4 && currentitem.UnitType == type) break;
                }
            }
            return dis;
        }
        public int GetLastRank4UpDistance(ulong id)
        {
            int idx = GetItemIndex(id);
            var logs_values = Logs.Values;
            DDCLGachaLogItem startitem = logs_values[idx];
            int dis = 0;
            for (int i = idx - 1; i >= 0; i--)
            {
                var currentitem = logs_values[i];
                if (currentitem.PoolType == startitem.PoolType)
                {
                    dis++;
                    if (currentitem.Rank == 4)
                    {
                        if (DDCL.BannerLib.GetBanner(currentitem.VersionID, currentitem.BannerInternalID)?.Rank4Up.Exists(x => x == currentitem.Name) ?? false) break;
                    }
                }
            }
            return dis;
        }


        public bool RebuildBasicLibrary()  // TODO
        {
            
            if (OriginalLogs == null)
            {
                //DDCLog.Warning(DCLN.Lib, String.Format("Failed to rebuild cache: nullptr"));
                // TODO: Signal
                return false;
            }

            Banners.Clear();
            BasicRounds.Clear();
            Logs.Clear();


            foreach (var b in DDCL.BannerLib.Banners)
            {
                DDCLBannerLogItem bannerlog = new DDCLBannerLogItem
                {
                    VersionID = b.VersionID,
                    BannerInternalID = b.InternalID,
                    CategorizedGachaType = b.Type,
                    GreaterRounds = new List<DDCLRoundLogItem>(),
                    Logs = new List<DDCLGachaLogItem>(),
                };
                Banners.Add(bannerlog);
            }


            foreach (var vlog in OriginalLogs.Logs.GroupBy(x => x.VersionID))
            {
                foreach (var blog in vlog.GroupBy(x => x.BannerInternalID))
                {
                    var t_bfst = blog.First();

                    var bannerlog = GetBanner(t_bfst.BannerInternalID);
                    foreach (var logitem in blog)
                    {
                        Logs.Add(logitem.ID, logitem);
                        bannerlog?.Logs.Add(logitem);
                    }
                }
            }

            //DDCLog.Info(DCLN.Lib, String.Format("Userdata cache rebuilded"));
            
            return true;
        }

        public bool RebuildGreaterRoundsLibrary()
        {
            
            GreaterRounds.Clear();
            var noup = new List<DDCCPoolType> { DDCCPoolType.Beginner, DDCCPoolType.Permanent };
            var haveup = new List<DDCCPoolType> { DDCCPoolType.CharacterEvent, DDCCPoolType.LCEvent };
            foreach (var type in noup)
            {
                var banners = GetBannersByCategorizedType(type);
                var buf = new List<DDCLGachaLogItem>();
                foreach (var bannerlog in banners)
                {
                    foreach (var roundlog in bannerlog.Logs.GroupBy(x => x.RoundID))
                    {
                        buf.AddRange(roundlog);
                        if (roundlog.Last().Rank == 5)
                        {
                            var greater = new DDCLRoundLogItem
                            {
                                BannerInternalID = bannerlog.BannerInternalID,
                                VersionID = bannerlog.VersionID,
                                CategorizedGachaType = bannerlog.CategorizedGachaType,
                                Logs = new List<DDCLGachaLogItem>(buf)
                            };
                            bannerlog.GreaterRounds.Add(greater);
                            GreaterRounds.Add(greater);
                            buf.Clear();
                        }
                    }
                    var greaterround = new DDCLRoundLogItem
                    {
                        BannerInternalID = bannerlog.BannerInternalID,
                        VersionID = bannerlog.VersionID,
                        CategorizedGachaType = bannerlog.CategorizedGachaType,
                        Logs = new List<DDCLGachaLogItem>(buf)
                    };
                    bannerlog.GreaterRounds.Add(greaterround);
                    GreaterRounds.Add(greaterround);
                }
            }

            foreach (var type in haveup)
            {
                var banners = GetBannersByCategorizedType(type);
                var buf = new List<DDCLRoundLogItem>();
                foreach (var bannerlog in banners)
                {
                    bannerlog.GreaterRounds.Clear();
                    var bannerlibinfo = DDCL.BannerLib.GetBanner(bannerlog.BannerInternalID);

                    if (buf.Count > 0)
                    {
                        int removecnt = 1 + buf.FindIndex(
                            x => x.Logs.Count > 0 && x.Logs.Last().Rank == 5
                            && (DDCL.BannerLib.GetBanner(x.BannerInternalID)?.Rank5Up.Contains(x.Logs.Last().Name) ?? false)
                        );
                        buf.RemoveRange(0, removecnt);
                    }
                    foreach (var roundlog in bannerlog.Logs.GroupBy(x => x.RoundID))
                    {
                        var t_rfst = roundlog.First();
                        var tmplogs = new DDCLRoundLogItem { BannerInternalID = t_rfst.BannerInternalID, Logs = new List<DDCLGachaLogItem>(roundlog.ToList()) };
                        buf.Add(tmplogs);

                        if (bannerlibinfo.Rank5Up.Contains(roundlog.Last().Name))
                        {
                            var greater = new DDCLRoundLogItem
                            {
                                VersionID = bannerlog.VersionID,
                                BannerInternalID = bannerlog.BannerInternalID,
                                CategorizedGachaType = bannerlog.CategorizedGachaType,
                                Logs = new List<DDCLGachaLogItem>(),
                            };
                            foreach (var r in buf)
                            {
                                greater.Logs.AddRange(r.Logs);
                            }
                            bannerlog.GreaterRounds.Add(greater);
                            GreaterRounds.Add(greater);
                            buf.Clear();
                        }

                    }
                    var greaterround = new DDCLRoundLogItem
                    {
                        VersionID = bannerlog.VersionID,
                        BannerInternalID = bannerlog.BannerInternalID,
                        CategorizedGachaType = bannerlog.CategorizedGachaType,
                        Logs = new List<DDCLGachaLogItem>()
                    };
                    foreach (var r in buf)
                    {
                        greaterround.Logs.AddRange(r.Logs);
                    }
                    bannerlog.GreaterRounds.Add(greaterround);
                    GreaterRounds.Add(greaterround);
                }
            }

            // TODO: Signal
            //DDCLog.Info(DCLN.Lib, String.Format("Round cache rebuilded"));
            
            return true;
        }

        public bool RebuildLibrary()
        {
            if (OriginalLogs == null)
            {
                return false;
            }
            return (RebuildBasicLibrary() && RebuildGreaterRoundsLibrary());
        }


        public bool SwapUser(DDCLUserGachaLog userlog)
        {
            if (userlog == null)
            {
                //DDCLog.Warning(DCLN.Lib, String.Format("Failed to swap user: nullptr"));
                return false;
            }
            if (userlog == OriginalLogs)
            {
                return true;
            }
            //DDCLog.Info(DCLN.Lib, String.Format("Swapping user. UID:{0}", userlog.uid));
            DDCS.Emit_CurUserSwapping(userlog.UID);
            var old = OriginalLogs;
            OriginalLogs = userlog;
            if (!RebuildLibrary())
            {
                OriginalLogs = old;
                //DDCLog.Info(DCLN.Lib, String.Format("Swap cancelled."));
                DDCS.Emit_CurUserSwapReverted();
                return false;
            }
            DDCS.Emit_CurUserSwapCompleted(userlog.UID);
            //DDCLog.Info(DCLN.Lib, String.Format("Swap completed."));
            return true;
        }

        public bool SwapUser(long uid)
        {
            return SwapUser(DDCL.UserDataLib.GetUserLogByUid(uid));
        }
        public async Task SaveUserAsync()
        {
            await DDCL.UserDataLib.SaveUserAsync(OriginalLogs);
        }

        public int GetActivatingTimeZone()
        {
            if (OriginalLogs == null) return DDCL.DefaultTimeZone;
            return OriginalLogs.TimeZone;
        }
        public bool IsCurrentUser(long uid)
            => uid == OriginalLogs.UID;
    }
}
