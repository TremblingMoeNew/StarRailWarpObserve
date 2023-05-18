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
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader
{
    public class DDCGUniversalFormatExporter
    {
        private ulong AnonymousID = 10000000000;



        public DDCGUniversalFormatLog CreateUFLog(long uid, bool anonymous)
        {
            if (!DDCL.UserDataLib.UserExists(uid)) return null;

            var userlog = DDCL.UserDataLib.GetUserLogByUid(uid);

            var res = new DDCGUniversalFormatLog
            {
                Info = CreateMetaInfo(userlog, anonymous),
                List = new List<DDCGUniversalFormatLogItem>(),
            };
            foreach(var item in userlog.Logs)
            {
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

        public DDCGUniversalFormatLogInfo CreateMetaInfo(DDCLUserGachaLog userlog, bool anonymous)
        {
            var info = new DDCGUniversalFormatLogInfo
            {
                UID = userlog.UID.ToString(),
                ApplicationVersion = DDCL.MetaVersionLib.ClientVersion,
                ExportTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now),
                ExportTimestamp = DDCL.ToUnixTimestamp(DateTime.Now).ToString(),
                TimeZone = userlog.TimeZone.ToString(),
                GameBiz = ConvertGameClientTypeToGameBizString(userlog.ClientType),

                Application = "DodocoTales.StarRail",
                StandardVersion = "v1.0",
                Game = "Honkai_Star_Rail",
            };
            if (anonymous)
            {
                info.UID = null;
                info.AnonymousExport = "true";
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

        public string GenerateExportFileName(long uid, bool anonymous)
        {
            return string.Format("StarwoExport_{1}_{0:yyyyMMdd_HHmmss}.json", DateTime.Now, anonymous ? "anonymous": uid.ToString());
        }

        public async Task<bool> Export(string filename, long uid, bool anonymous)
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
                var serialized = JsonConvert.SerializeObject(CreateUFLog(uid, anonymous), Formatting.Indented);
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
