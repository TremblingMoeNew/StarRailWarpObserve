using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Library.UserDataLibrary.Models;
using DodocoTales.SR.Loader.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader
{
    public class DDCGUniversalFormatImporter
    {
        public async Task<DDCGUniversalFormatLog> Load(string filepath)
        {
            try
            {
                var stream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(stream);
                var buffer = await reader.ReadToEndAsync();
                return EnsureToNewFormat(JsonConvert.DeserializeObject<DDCGUniversalFormatLog>(buffer));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DDCGUniversalFormatLog EnsureToNewFormat(DDCGUniversalFormatLog log)
        {
            if (log.Info == null) return null;

            if (log.StarRailSections == null)
            {
                if (log.List == null) return null;

                log.StarRailSections = new List<DDCGUniversalFormatLogSRSection>
                {
                    new DDCGUniversalFormatLogSRSection
                    {
                        UID = log.Info.UID,
                        Language = log.Info.Language,
                        TimeZone = log.Info.TimeZone,
                        GameBiz = log.Info.GameBiz,
                        List = log.List
                    }
                };
            }
            return log;
        }


        public bool IsAcceptableFormat(DDCGUniversalFormatLog log)
            => ((log?.Info?.LegacyStandardVersion != null && log?.Info.UID != null) || (log?.Info?.NewStandardVersion != null) || (log?.Info?.AnonymousExport == "true")) && log?.StarRailSections != null;

        public bool IsAcceptableSection(DDCGUniversalFormatLogSRSection section)
            => (section.UID != null) && (section?.List != null);

        public bool IsAcceptableLanguage(DDCGUniversalFormatLogSRSection section)
             => section?.Language == "zh-cn";

        public bool IsAnonymousFormat(DDCGUniversalFormatLog log)
                => log?.Info?.AnonymousExport == "true";


        public DDCLGameClientType ConvertGameBizStringToGameClientType(string game_biz)
        {
            switch (game_biz)
            {
                case "hkrpg_cn":
                    return DDCLGameClientType.CN;
                case "hkrpg_global":
                    return DDCLGameClientType.Global;
                default:
                    return DDCLGameClientType.Unknown;
            }
        }
        public DDCLGachaLogItem ConvertToDDCLLogItem(DDCGUniversalFormatLogItem UFItem)
        {
            var item = new DDCLGachaLogItem()
            {
                Name = UFItem.name,
                Raw = new DDCLGachaLogItemRawData
                {
                    count = UFItem.count,
                    gacha_id = UFItem.gacha_id,
                    item_id = UFItem.item_id,
                },
            };

            try
            {
                if (UFItem.id == null || UFItem.rank_type == null || UFItem.time == null) return null;
                if (UFItem.item_type == null || UFItem.gacha_type == null) return null;

                item.ID = Convert.ToUInt64(UFItem.id);
                item.Rank = Convert.ToUInt64(UFItem.rank_type);
                item.Time = Convert.ToDateTime(UFItem.time);
                item.UnitType = DDCL.ConvertToUnitType(UFItem.item_type);
                item.Raw.gacha_type = Convert.ToUInt64(UFItem.gacha_type);
                item.PoolType = JsonConvert.DeserializeObject<DDCCPoolType>(UFItem.gacha_type);

                if (item.Rank < 3 || item.Rank > 5) return null;
                if (item.UnitType == DDCCUnitType.Unknown) return null;
            }
            catch
            {
                return null;
            }
            return item;
        }

        public List<DDCLGachaLogItem> ConvertList(List<DDCGUniversalFormatLogItem> UFList)
        {
            var res = new SortedList<ulong, DDCLGachaLogItem>();
            foreach(var UFItem in UFList)
            {
                var item = ConvertToDDCLLogItem(UFItem);
                if (item != null) res.Add(item.ID, item);
            }
            return res.Values.ToList();
        }

        public int Import(long uid, List<DDCLGachaLogItem> loglist, DDCLGameClientType clientType, int timezone,string export_app)
        {
            var userlog = DDCL.UserDataLib.GetUserLogByUid(uid);
            var merger = new DDCGGachaLogMerger(userlog);
            if (userlog.ClientType == DDCLGameClientType.Unknown)
            {
                userlog.ClientType = clientType;
                merger.SetTimeZone(timezone);
            }
            loglist.ForEach(log => { log.Imported = true; log.ImportApplication = export_app; });
            return merger.Merge(loglist, false);
        }
    }
}
