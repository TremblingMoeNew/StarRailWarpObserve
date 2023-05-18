using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.BannerLibrary.Models;
using DodocoTales.SR.Library.UserDataLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader
{
    public class DDCGGachaLogMerger
    {
        public SortedList<ulong, DDCLGachaLogItem> GachaLogSet;
        DDCLUserGachaLog UserLog;
        public DDCGGachaLogMerger(DDCLUserGachaLog userlog)
        {
            UserLog = userlog;
            GachaLogSet = new SortedList<ulong, DDCLGachaLogItem>();
            foreach (var item in userlog.Logs)
            {
                GachaLogSet.Add(item.ID, item);
            }
        }
        public void SetTimeZone(int timezone)
        {
            UserLog.TimeZone = timezone;
        }

        public ulong GetLastItemMihoyoIdByType(DDCCPoolType type)
        {
            var last = GachaLogSet.Values.LastOrDefault(x => x.PoolType == type);
            return last == null ? 0 : last.ID;
        }

        public int MergeLogToDict(List<DDCLGachaLogItem> imported, bool replace)
        {
            int cnt = 0;
            foreach(var item in imported)
            {
                if(replace || !GachaLogSet.ContainsKey(item.ID))
                {
                    GachaLogSet[item.ID] = item;
                    cnt++;
                }
            }
            return cnt;
        }

        public void RebuildClassifiers()
        {
            int versionidx = 0, versioncnt = DDCL.BannerLib.Versions.Count;
            if (versioncnt == 0) return;
            var version = DDCL.BannerLib.Versions[versionidx];
            Dictionary<DDCCPoolType, int> banneridx = new Dictionary<DDCCPoolType, int>
            {
                { DDCCPoolType.Beginner, 0 },
                { DDCCPoolType.Permanent, 0 },
                { DDCCPoolType.CharacterEvent, 0 },
                { DDCCPoolType.LCEvent, 0 }
            };
            Dictionary<DDCCPoolType, ulong> round = new Dictionary<DDCCPoolType, ulong>
            {
                { DDCCPoolType.Beginner, 0 },
                { DDCCPoolType.Permanent, 0 },
                { DDCCPoolType.CharacterEvent, 0 },
                { DDCCPoolType.LCEvent, 0 }
            };
            Dictionary<DDCCPoolType, ulong> round_replacing = new Dictionary<DDCCPoolType, ulong>
            {
                { DDCCPoolType.Beginner, 0 },
                { DDCCPoolType.Permanent, 0 },
                { DDCCPoolType.CharacterEvent, 0 },
                { DDCCPoolType.LCEvent, 0 }
            };
            foreach (var item in GachaLogSet.Values)
            {
                if (round[item.PoolType] == 0) round[item.PoolType] = item.ID;
                if (item.RoundID != round[item.PoolType])
                {
                    if (item.RoundID != 0 && item.RoundID != round_replacing[item.PoolType])
                    {
                        round_replacing[item.PoolType] = item.RoundID;
                    }
                    item.RoundID = round[item.PoolType];

                }
                if (item.Rank == 5) round[item.PoolType] = 0;
                if (item.VersionID == 0)
                {
                    while (version != null && CheckTimeWithVersion(version, item.Time) < 0)
                    {
                        versionidx++;
                        if (versioncnt > versionidx) version = DDCL.BannerLib.Versions[versionidx]; else version = null;
                        banneridx[DDCCPoolType.Beginner] = 0;
                        banneridx[DDCCPoolType.Permanent] = 0;
                        banneridx[DDCCPoolType.CharacterEvent] = 0;
                        banneridx[DDCCPoolType.LCEvent] = 0;
                    }
                    if (version == null || CheckTimeWithVersion(version, item.Time) > 0)
                    {
                        //DDCLog.Error(DCLN.Loader, String.Format("Log item classification failed. {0}", JsonConvert.SerializeObject(item)));
                        continue;
                    }
                    item.VersionID = version.ID;
                    if (item.BannerInternalID == 0)
                    {
                        var bannercnt = version.Banners.Count;
                        var bidx = banneridx[item.PoolType];
                        while (bidx < bannercnt && (version.Banners[bidx].Type != item.PoolType
                            || CheckTimeWithBanner(version.Banners[bidx], item.Time) < 0)) bidx++;
                        banneridx[item.PoolType] = bidx;
                        var banner = bidx < bannercnt ? version.Banners[bidx] : null;
                        if (banner == null || CheckTimeWithBanner(banner, item.Time) > 0)
                        {
                            //DDCLog.Error(DCLN.Loader, String.Format("Log item classification failed. {0}", JsonConvert.SerializeObject(item)));
                            continue;
                        }
                        item.BannerInternalID = banner.InternalID;

                    }
                }
            }
        }
        public void WriteToUserlog()
        {
            RebuildClassifiers();
            UserLog.Logs = GachaLogSet.Values.ToList();
            DDCL.UserDataLib.SaveUserAsync(UserLog);
            if (DDCL.CurrentUser.IsCurrentUser(UserLog.UID))
            {
                DDCL.CurrentUser.RebuildLibrary();
            }
        }

        public int Merge(List<DDCLGachaLogItem> imported, bool replace)
        {
            var cnt = MergeLogToDict(imported, replace);
            WriteToUserlog();
            return cnt;
        }
        public int CheckTimeWithVersion(DDCLVersionInfo version, DateTime time)
        {
            var item = DDCL.GetTimeOffset(time, UserLog.TimeZone);
            var version_begin = DDCL.GetSyncTimeOffset(version.BeginTime);
            var version_end = DDCL.GetSyncTimeOffset(version.EndTime);
            return DDCL.CheckTimeIsBetween(version_begin, version_end, item);
        }
        public int CheckTimeWithBanner(DDCLBannerInfo banner, DateTime time)
        {
            var item = DDCL.GetTimeOffset(time, UserLog.TimeZone);
            var banner_begin = DDCL.GetBannerTimeOffset(banner.BeginTime, banner.BeginTimeSync, UserLog.TimeZone);
            var banner_end = DDCL.GetBannerTimeOffset(banner.EndTime, banner.EndTimeSync, UserLog.TimeZone);
            return DDCL.CheckTimeIsBetween(banner_begin, banner_end, item);
        }

    }
}
