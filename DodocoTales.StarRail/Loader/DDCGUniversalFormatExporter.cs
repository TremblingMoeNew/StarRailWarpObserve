using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Library.UserDataLibrary.Models;
using DodocoTales.SR.Loader.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader
{
    public class DDCGUniversalFormatExporter
    {
        private ulong AnonymousID = 10000000000;



        public DDCGUniversalFormatLog CreateUFLog(List<long> UIDList, bool new_support, bool legacy_support, bool anonymous)
        {
            if (UIDList == null || UIDList.Count == 0) return null;
            DDCLUserGachaLog legacy_userlog = null;

            if (legacy_support)
            {
                if (!DDCL.UserDataLib.UserExists(UIDList.First())) return null;
                legacy_userlog = DDCL.UserDataLib.GetUserLogByUid(UIDList.First());
            }
            var res = new DDCGUniversalFormatLog
            {
                Info = CreateMetaInfo(legacy_userlog, new_support, legacy_support, anonymous),
            };
            var sections = new List<DDCGUniversalFormatLogSRSection>();
            foreach (var uid in UIDList)
            {
                if (!DDCL.UserDataLib.UserExists(uid)) continue;
                sections.Add(ExportUser(DDCL.UserDataLib.GetUserLogByUid(uid), anonymous));
            }
            if (new_support)
            {
                res.StarRailSections = sections;
            }
            if (legacy_support)
            {
                if (sections.Count == 0) return null;
                res.List = sections.First().List;
            }
            return res;
        }


        public DDCGUniversalFormatLogSRSection ExportUser(DDCLUserGachaLog userlog, bool anonymous)
        {
            var res = new DDCGUniversalFormatLogSRSection
            {
                UID = userlog.UID.ToString(),
                TimeZone = userlog.TimeZone.ToString(),
                GameBiz = ConvertGameClientTypeToGameBizString(userlog.ClientType),
                Language = "zh-cn",
                List = new List<DDCGUniversalFormatLogItem>(),
            };
            foreach (var item in userlog.Logs)
            {
                if (item.Untrusted) continue;
                res.List.Add(ConvertToUFItem(item, anonymous));
            }
            return res;
        }



        public DDCGUniversalFormatLogItem ConvertToUFItem(DDCLGachaLogItem item, bool anonymous)
        {
            var res = new DDCGUniversalFormatLogItem
            {
                name = item.Name,
                id = item.ID.ToString(),
                item_type = DDCL.ConvertUnitTypeToString(item.UnitType),
                rank_type = item.Rank.ToString(),
                time = String.Format("{0:yyyy-MM-dd HH:mm:ss}", item.Time),
                count = item.Raw.count,
                item_id = item.Raw.item_id,
                gacha_id = item.Raw.gacha_id,
                gacha_type = item.Raw.gacha_type.ToString(),
            };

            if (anonymous == true)
            {
                res.__hash = DDCL.MD5Hash(JsonConvert.SerializeObject(res));
                res.id = AnonymousID++.ToString();
            }
            return res;
        }

        public DDCGUniversalFormatLogInfo CreateMetaInfo(DDCLUserGachaLog userlog, bool new_support, bool legacy_support, bool anonymous)
        {
            var info = new DDCGUniversalFormatLogInfo
            {
                Application = "DodocoTales.StarRail",
                ApplicationVersion = DDCL.MetaVersionLib.ClientVersion,
                ExportTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now),
                ExportTimestamp = DDCL.ToUnixTimestamp(DateTime.Now).ToString(),
            };
            if (new_support)
            {
                info.NewStandardVersion = "v4.0";
            }
            if (legacy_support)
            {
                info.UID = userlog.UID.ToString();
                info.LegacyStandardVersion = "v1.0";
                info.TimeZone = userlog.TimeZone.ToString();
                info.GameBiz = ConvertGameClientTypeToGameBizString(userlog.ClientType);
                info.Game = "Honkai_Star_Rail";
                info.Language = "zh-cn";
                if (anonymous)
                {
                    info.UID = null;
                    info.AnonymousExport = "true";
                }
            }

            return info;
        }

        public string ConvertGameClientTypeToGameBizString(DDCLGameClientType client)
        {
            switch(client)
            {
                case DDCLGameClientType.CN:
                    return "hkrpg_cn";
                case DDCLGameClientType.Global:
                    return "hkrpg_global";
                default:
                    return null;
            }
        }

        public string GenerateLegacyExportFileName(long uid, bool anonymous = false)
        {
            return string.Format("StarwoExport_Legacy_{1}_{0:yyyyMMdd_HHmmss}.json", DateTime.Now, anonymous ? "anonymous": uid.ToString());
        }

        public string GenerateMultiExportFileName()
        {
            return string.Format("StarwoExport_NUIGF_Multi_{0:yyyyMMdd_HHmmss}.json", DateTime.Now);
        }

        public string GenerateDualExportFileName(long uid, bool anonymous = false)
        {
            return string.Format("StarwoExport_Dual_{1}_{0:yyyyMMdd_HHmmss}.json", DateTime.Now, anonymous ? "anonymous" : uid.ToString());
        }

        public async Task<bool> Export(string filename, List<long> UIDList, bool new_support, bool legacy_support, bool anonymous)
        {
            FileInfo fileinfo = new FileInfo(filename);
            try
            {
                if (!fileinfo.Directory.Exists)
                {
                    Directory.CreateDirectory(fileinfo.DirectoryName);
                }
                var stream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);
                var serialized = JsonConvert.SerializeObject(CreateUFLog(UIDList,new_support, legacy_support, anonymous), Formatting.Indented);
                await writer.WriteAsync(serialized);
                await writer.FlushAsync();
                stream.Close();
                //DDCLog.Info(DCLN.Loader, String.Format("Userlog exported as universal format. UID:{0}", uid));
            }
            catch (Exception e)
            {
                return false;
                //DDCLog.Error(DCLN.Loader, String.Format("Failed to exported userlog. UID:{0}", uid), e);
            }
            
            Process.Start(fileinfo.DirectoryName);
            return true;
        }

    }
}
