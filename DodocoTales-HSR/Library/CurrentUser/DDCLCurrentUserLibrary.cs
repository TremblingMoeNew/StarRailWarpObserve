using DodocoTales.SR.Common;
using DodocoTales.SR.Common.Enums;
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
        /*
        public List<DDCLRoundLogItem> BasicRounds { get; set; }
        public List<DDCLRoundLogItem> GreaterRounds { get; set; }
        public List<DDCLBannerLogItem> Banners { get; set; }
        */

        public DDCLCurrentUserLibrary()
        {
            Logs = new SortedList<ulong, DDCLGachaLogItem>();
            /*
            BasicRounds = new List<DDCLRoundLogItem>();
            GreaterRounds = new List<DDCLRoundLogItem>();
            Banners = new List<DDCLBannerLogItem>();
            */
        }

        /*
        // 查找
        // Banners
        public DDCLBannerLogItem GetBanner(ulong versionid, ulong bannerid)
            => Banners.Find(x => x.VersionId == versionid && x.BannerId == bannerid);
        public int GetBannerIndex(ulong versionid, ulong bannerid)
            => Banners.FindIndex(x => x.VersionId == versionid && x.BannerId == bannerid);
        public List<DDCLBannerLogItem> GetBannersByVersion(ulong versionid)
            => Banners.FindAll(x => x.VersionId == versionid);
        public List<DDCLBannerLogItem> GetBannersByCategorizedType(DDCCPoolType type)
            => Banners.FindAll(x => x.CategorizedGachaType == type);
        public List<DDCLBannerLogItem> GetBannersByVersionAndType(ulong versionid, DDCCPoolType type)
            => Banners.FindAll(x => x.VersionId == versionid && x.CategorizedGachaType == type);
        // GreaterRounds
        // BasicRounds

        */
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
                        if (DDCL.BannerLib.GetBanner(currentitem.VersionID, currentitem.BannerID)?.Rank4Up.Exists(x => x == currentitem.Name) ?? false) break;
                    }
                }
            }
            return dis;
        }


        public bool RebuildBasicLibrary()  // TODO
        {
            /*
            if (OriginalLogs == null)
            {
                DDCLog.Warning(DCLN.Lib, String.Format("Failed to rebuild cache: nullptr"));
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
                    VersionId = b.VersionId,
                    BannerId = b.id,
                    CategorizedGachaType = b.type,
                    GreaterRounds = new List<DDCLRoundLogItem>(),
                    Logs = new List<DDCLGachaLogItem>(),
                };
                Banners.Add(bannerlog);
            }


            foreach (var vlog in OriginalLogs.Logs.GroupBy(x => x.version_id))
            {
                foreach (var blog in vlog.GroupBy(x => x.banner_id))
                {
                    int t_idx = 0;
                    var t_bfst = blog.First();

                    var bannerlog = GetBanner(t_bfst.version_id, t_bfst.banner_id);
                    foreach (var logitem in blog)
                    {
                        Logs.Add(logitem.internal_id, logitem); ;
                        bannerlog?.Logs.Add(logitem);
                    }
                }
            }

            DDCLog.Info(DCLN.Lib, String.Format("Userdata cache rebuilded"));
            */
            return true;
        }

        public bool RebuildGreaterRoundsLibrary()
        {
            /*
            GreaterRounds.Clear();
            var noup = new List<DDCCPoolType> { DDCCPoolType.Beginner, DDCCPoolType.Permanent };
            var haveup = new List<DDCCPoolType> { DDCCPoolType.EventCharacter, DDCCPoolType.EventWeapon };
            foreach (var type in noup)
            {
                var banners = GetBannersByCategorizedType(type);
                var buf = new List<DDCLGachaLogItem>();
                foreach (var bannerlog in banners)
                {
                    foreach (var roundlog in bannerlog.Logs.GroupBy(x => x.round_id))
                    {
                        buf.AddRange(roundlog);
                        if (roundlog.Last().rank == 5)
                        {
                            var greater = new DDCLRoundLogItem
                            {
                                BannerId = bannerlog.BannerId,
                                VersionId = bannerlog.VersionId,
                                CategorizedGachaType = bannerlog.CategorizedGachaType,
                                EpitomizedPathID = 0,
                                Logs = new List<DDCLGachaLogItem>(buf)
                            };
                            bannerlog.GreaterRounds.Add(greater);
                            GreaterRounds.Add(greater);
                            buf.Clear();
                        }
                    }
                    var greaterround = new DDCLRoundLogItem
                    {
                        BannerId = bannerlog.BannerId,
                        VersionId = bannerlog.VersionId,
                        CategorizedGachaType = bannerlog.CategorizedGachaType,
                        EpitomizedPathID = 0,
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
                    var bannerlibinfo = DDCL.BannerLib.GetBanner(bannerlog.VersionId, bannerlog.BannerId);
                    var EPExtended = false;

                    if (buf.Count > 0)
                    {
                        int removecnt = 1 + buf.FindIndex(
                            x => x.Logs.Count > 0 && x.Logs.Last().rank == 5
                            && (DDCL.BannerLib.GetBanner(x.VersionId, x.BannerId)?.rank5Up.Contains(x.Logs.Last().unitclass) ?? false)
                        );
                        buf.RemoveRange(0, removecnt);
                    }
                    if (buf.Count > 0)
                    {
                        EPExtended = true;
                    }
                    var epl_ls = OriginalLogs.EpitomizedPath.FindAll(x => x.version_id == bannerlog.VersionId && x.banner_id == bannerlog.BannerId);
                    int epl_ptr = 0, epl_len = epl_ls.Count;
                    var epl = epl_len > 0 ? epl_ls[epl_ptr++] : new DDCLEpitomizedPathItem { enabled = false, unitclass = 0, round_id = ulong.MaxValue };
                    foreach (var roundlog in bannerlog.Logs.GroupBy(x => x.round_id))
                    {
                        var t_rfst = roundlog.First();
                        while (epl_ptr < epl_len && t_rfst.round_id >= epl_ls[epl_ptr].round_id) epl = epl_ls[epl_ptr++];
                        if (buf.Count > 0)
                        {
                            if (buf.Last().BannerId == t_rfst.banner_id
                                && buf.Last().EpitomizedPathID != 0
                                && buf.Last().EpitomizedPathID != epl.unitclass
                                )
                            {
                                int removecnt = 1 + buf.FindIndex(
                                    x => x.Logs.Count > 0 && x.Logs.Last().rank == 5
                                    && (DDCL.BannerLib.GetBanner(x.VersionId, x.BannerId)?.rank5Up.Contains(x.Logs.Last().unitclass) ?? false)
                                );

                                var greater = new DDCLRoundLogItem
                                {
                                    VersionId = bannerlog.VersionId,
                                    BannerId = bannerlog.BannerId,
                                    CategorizedGachaType = bannerlog.CategorizedGachaType,
                                    EpitomizedPathID = buf[removecnt - 1].EpitomizedPathID,
                                    Logs = new List<DDCLGachaLogItem>(),
                                    IsEPExtendedRound = false
                                };
                                foreach (var r in buf.GetRange(0, removecnt))
                                {
                                    greater.Logs.AddRange(r.Logs);
                                }
                                bannerlog.GreaterRounds.Add(greater);
                                GreaterRounds.Add(greater);

                                buf.RemoveRange(0, removecnt);
                                EPExtended = true;
                            }
                        }
                        var tmplogs = new DDCLRoundLogItem { BannerId = t_rfst.banner_id, EpitomizedPathID = epl.unitclass, Logs = new List<DDCLGachaLogItem>(roundlog.ToList()) };
                        buf.Add(tmplogs);

                        if (bannerlibinfo.rank5Up.Contains(roundlog.Last().unitclass))
                        {
                            if (tmplogs.EpitomizedPathID == 0 || tmplogs.EpitomizedPathID == tmplogs.Logs.Last().unitclass)
                            {
                                if (tmplogs.EpitomizedPathID == 0)
                                {
                                    EPExtended = false;
                                }

                                var greater = new DDCLRoundLogItem
                                {
                                    VersionId = bannerlog.VersionId,
                                    BannerId = bannerlog.BannerId,
                                    CategorizedGachaType = bannerlog.CategorizedGachaType,
                                    EpitomizedPathID = tmplogs.EpitomizedPathID,
                                    Logs = new List<DDCLGachaLogItem>(),
                                    IsEPExtendedRound = EPExtended
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

                    }
                    var greaterround = new DDCLRoundLogItem
                    {
                        VersionId = bannerlog.VersionId,
                        BannerId = bannerlog.BannerId,
                        CategorizedGachaType = bannerlog.CategorizedGachaType,
                        EpitomizedPathID = buf.Count > 0 ? buf.Last().EpitomizedPathID : 0,
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
            DDCLog.Info(DCLN.Lib, String.Format("Round cache rebuilded"));
            */
            return true;
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
            if (!RebuildBasicLibrary())
            {
                OriginalLogs = old;
                //DDCLog.Info(DCLN.Lib, String.Format("Swap cancelled."));
                DDCS.Emit_CurUserSwapReverted();
                return false;
            }
            RebuildGreaterRoundsLibrary();
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
    }
}
