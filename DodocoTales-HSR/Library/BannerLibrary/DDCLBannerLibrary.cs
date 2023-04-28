using DodocoTales.SR.Common;
using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Library.BannerLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.BannerLibrary
{
    public class DDCLBannerLibrary
    {
        public readonly string libPath = @"library/BannerLibrary.json";
        DDCLBannerLibModel model;

        private DDCLVersionInfo defaultVersion;
        private DDCLBannerInfo defaultBanner;

        public List<DDCLVersionInfo> Versions { get; set; }
        public List<DDCLBannerInfo> Banners { get; set; }

        public DDCLBannerLibrary()
        {
            Versions = new List<DDCLVersionInfo>();
            Banners = new List<DDCLBannerInfo>();
            defaultVersion = new DDCLVersionInfo { Name = "未知", Version = "---", ID = 0 };
            defaultBanner = new DDCLBannerInfo { Name = "未知", Hint = "未知", Type = DDCCPoolType.Null, ID = 0,  VersionId = 0, Rank4Up = new List<string>(), Rank5Up = new List<string>() };
        }

        public DDCLVersionInfo GetVersion(ulong versionid)
            => versionid == 0 ? defaultVersion : Versions.Find(x => x.ID == versionid);

        public DDCLBannerInfo GetBanner(ulong versionid, ulong bannerid)
            => GetBanner(ConvertToInternalBannerId(versionid, bannerid));

        public DDCLBannerInfo GetBanner(ulong internalbannerid)
            => internalbannerid == 0 ? defaultBanner : Banners.Find(x => x.InternalId == internalbannerid);

        public List<DDCLBannerInfo> GetBannersByType(DDCCPoolType type)
            => Banners.FindAll(x => x.Type == type);
        public List<DDCLBannerInfo> GetBannersByR5Up(string name)
            => Banners.FindAll(x => x.Rank5Up.Contains(name));
        public List<DDCLBannerInfo> GetBannersByR4Up(string name)
            => Banners.FindAll(x => x.Rank4Up.Contains(name));

        public ulong ConvertToInternalBannerId(ulong versionid, ulong bannerid)
            => versionid * 1000 + bannerid;


        public async Task<bool> LoadModelAsync()
        {
            try
            {
                var stream = File.Open(libPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(stream);
                var buffer = await reader.ReadToEndAsync();
                model = JsonConvert.DeserializeObject<DDCLBannerLibModel>(buffer);
            }
            catch (Exception)
            {
                model = null;
                //DDCLog.Error(DCLN.Lib, "Failed to load bannerlib.");
                DDCS.Emit_BannerLibReloadFailed();
                return false;
            }
            //DDCLog.Info(DCLN.Lib, "Bannerlib deserialized.");
            DDCS.Emit_BannerLibDeserialized();
            return true;
        }

        public bool RebuildLibrary()
        {
            if (model == null)
            {
                return false;
            }
            var beginners = model.BeginnerPools;
            var permanents = model.PermanentPools;
            int bpidx = 0, ppidx = 0;
            int bplen = beginners.Count, pplen = permanents.Count;
            foreach (var version in model.EventPools)
            {
                Versions.Add(version);
                while (ppidx < pplen && DateTime.Compare(permanents[ppidx].EndTime, version.BeginTime) < 0) ppidx++;
                while (ppidx < pplen && DateTime.Compare(permanents[ppidx].BeginTime, version.EndTime) < 0)
                {
                    var new_permanenent = permanents[ppidx].Copy();
                    if (DateTime.Compare(permanents[ppidx].BeginTime, version.BeginTime) < 0)
                    {
                        new_permanenent.BeginTime = version.BeginTime;
                        new_permanenent.BeginTimeSync = true;
                    }
                    if (DateTime.Compare(permanents[ppidx].EndTime, version.EndTime) > 0)
                    {
                        new_permanenent.EndTime = version.EndTime;
                        new_permanenent.EndTimeSync = true;
                    }
                    version.Banners.Insert(0, new_permanenent);
                    ppidx++;
                }
                ppidx--;
                while (bpidx < bplen && DateTime.Compare(beginners[ppidx].EndTime, version.BeginTime) < 0) bpidx++;
                while (bpidx < bplen && DateTime.Compare(beginners[bpidx].BeginTime, version.EndTime) < 0)
                {
                    var new_beginner = beginners[bpidx].Copy();
                    if (DateTime.Compare(beginners[bpidx].BeginTime, version.BeginTime) < 0)
                    {
                        new_beginner.BeginTime = version.BeginTime;
                        new_beginner.BeginTimeSync = true;
                    }
                    if (DateTime.Compare(beginners[bpidx].EndTime, version.EndTime) > 0)
                    {
                        new_beginner.EndTime = version.EndTime;
                        new_beginner.EndTimeSync = true;
                    }
                    version.Banners.Insert(0, new_beginner);
                    bpidx++;
                }
                bpidx--;

                foreach (var banner in version.Banners)
                {
                    banner.VersionId = version.ID;
                    Banners.Add(banner);
                }
            }

            return true;
        }
        public async Task<bool> LoadLibraryAsync()
        {
            Versions.Clear();
            Banners.Clear();

            if (!await LoadModelAsync())
            {
                return false;
            }
            RebuildLibrary();
            DDCS.Emit_BannerLibReloadCompleted();
            return true;
        }
    }
}
