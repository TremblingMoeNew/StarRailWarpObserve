using DodocoTales.SR.Common;
using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Library.UserDataLibrary.Models;
using DodocoTales.SR.Loader.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader
{
    public partial class DDCGWebGachaLogLoader
    {

        public DDCLGameClientType ClientType;
        public string ApiPattern
        {
            get
            {
                if (ClientType == DDCLGameClientType.CN)
                    return apipattern_cn;
                else
                    return null;
            }
        }
        readonly string apipattern_cn = @"https://api-takumi.mihoyo.com/common/gacha_record/api/getGachaLog?{0}&gacha_type={1}&page={2}&size={4}&end_id={3}";

        HttpClient client;

        public DDCGWebGachaLogLoader()
        {
            client = new HttpClient();
        }

        public async Task<List<DDCGGachaLogResponseItem>> GetGachaLogAsync(string authkey, int pageid, DDCCPoolType type, ulong lastid, int size = 6, int retrycnt = 0)
        {
            if (ApiPattern == null)
            {
                return null;
            }
            var api = String.Format(
                ApiPattern,
                authkey, (int)type, pageid, lastid, size
            );
            Thread.Sleep(200);
            string stringresponse;
            try
            {
                stringresponse = await client.GetStringAsync(api);
            }
            catch
            {
                //DDCLog.Warning(DCLN.Loader, "MiHoYo-API connection timeout.");
                if (retrycnt == 3) return null;
                DDCS.Emit_ImportConnectionTimeout();
                Thread.Sleep(1000);
                DDCS.Emit_ImportConnectionRetry();
                return await GetGachaLogAsync(authkey, pageid, type, lastid, size);
            }
            var response = JsonConvert.DeserializeObject<DDCGGachaLogResponse>(stringresponse);
            if (response.retcode != 0)
            {
                if (response.retcode == -110)
                {
                    //DDCLog.Warning(DCLN.Loader, "MiHoYo-API connection throttled.");
                    if (retrycnt > 10) return null;
                    DDCS.Emit_ImportConnectionThrottled();
                    if (retrycnt > 5)
                    {
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(1000);
                    DDCS.Emit_ImportConnectionRetry();
                    return await GetGachaLogAsync(authkey, pageid, type, lastid, size);
                }
                //DDCLog.Warning(DCLN.Loader, String.Format("Unknown retcode {0}: {1}", response.retcode, response.message));
                return null;
            }
            return response.data.list;
        }

        public async Task<List<DDCGGachaLogResponseItem>> GetGachaLogsByTypeAsync(string authkey, DDCCPoolType type, ulong endid, ulong beginid = 0, int size = 5)
        {
            var res = new List<DDCGGachaLogResponseItem>();
            //DDCLog.Info(DCLN.Loader, String.Format("Fetching gachalogs in {0} pool of {1}-{2}", type, endid, beginid));
            ulong lastid = beginid;
            bool run = true;
            for (int i = 1; run; i++)
            {
                DDCS.Emit_ImportStatusFromWebRefreshed(type, i);
                var list = await GetGachaLogAsync(authkey, i, type, lastid, size);
                if (list == null)
                {
                    return res;
                }
                foreach (var item in list)
                {
                    lastid = item.id;
                    if (lastid == endid)
                    {
                        run = false;
                        break;
                    }
                    res.Add(item);
                }
                if (list.Count < size) run = false;
            }
            res.Reverse();
            return res;
        }


        public async Task<long> TryConnectAndGetUid(string authkey, DDCLGameClientType clientType)
        {
            ClientType = clientType;
            var uid = await GetUidFromWeb(authkey);
            return uid;
        }

        public async Task<long> GetUidFromWeb(string authkey)
        {
            var bl = await GetGachaLogAsync(authkey, 1, DDCCPoolType.Beginner, 0);
            if (bl == null) return -2;
            bl.AddRange(await GetGachaLogAsync(authkey, 1, DDCCPoolType.Permanent, 0));
            if (bl.Count == 0)
            {
                bl.AddRange(await GetGachaLogAsync(authkey, 1, DDCCPoolType.CharacterEvent, 0));
                bl.AddRange(await GetGachaLogAsync(authkey, 1, DDCCPoolType.LCEvent, 0));
            }
            if (bl.Count == 0)
                return -1;
            else
                return bl[0].uid;
        }
        
        public DDCLGachaLogItem ConvertToDDCLLogItem(DDCGGachaLogResponseItem DDCGItem, DDCCPoolType pooltype)
        {
            return new DDCLGachaLogItem()
            {
                ID = DDCGItem.id,
                Name = DDCGItem.name,
                Rank = DDCGItem.rank_type,
                Time = DDCGItem.time,
                UnitType = DDCL.ConvertToUnitType(DDCGItem.item_type),
                PoolType = pooltype,
                Raw = new DDCLGachaLogItemRawData
                {
                    count = DDCGItem.count,
                    gacha_id = DDCGItem.gacha_id,
                    gacha_type = DDCGItem.gacha_type,
                    item_id = DDCGItem.item_id,
                },
            };
        }
        

        public async Task GetGachaLogsAsNormalMode(string authkey, DDCLGameClientType clientType)
        {
            
            ClientType = clientType;
            var merger = new DDCGGachaLogMerger(DDCL.CurrentUser.OriginalLogs);
            var res = new SortedList<ulong, DDCLGachaLogItem>();
            foreach (var item in await GetGachaLogsByTypeAsync(authkey, DDCCPoolType.Beginner, merger.GetLastItemMihoyoIdByType(DDCCPoolType.Beginner)))
            {
                res.Add(item.id, ConvertToDDCLLogItem(item, DDCCPoolType.Beginner));
            }
            foreach (var item in await GetGachaLogsByTypeAsync(authkey, DDCCPoolType.Permanent, merger.GetLastItemMihoyoIdByType(DDCCPoolType.Permanent)))
            {
                res.Add(item.id, ConvertToDDCLLogItem(item, DDCCPoolType.Permanent));
            }
            foreach (var item in await GetGachaLogsByTypeAsync(authkey, DDCCPoolType.CharacterEvent, merger.GetLastItemMihoyoIdByType(DDCCPoolType.CharacterEvent)))
            {
                res.Add(item.id, ConvertToDDCLLogItem(item, DDCCPoolType.CharacterEvent));
            }
            foreach (var item in await GetGachaLogsByTypeAsync(authkey, DDCCPoolType.LCEvent, merger.GetLastItemMihoyoIdByType(DDCCPoolType.LCEvent)))
            {
                res.Add(item.id, ConvertToDDCLLogItem(item, DDCCPoolType.LCEvent));
            }
            merger.Merge(res.Values.ToList(), true);
            
        }


        public async Task GetGachaLogsAsFullMode(string authkey, DDCLGameClientType clientType)
        {
            
            ClientType = clientType;
            //var merger = new DDCGGachaLogMerger(DDCL.CurrentUser.OriginalLogs);
            var res = new SortedList<ulong, DDCLGachaLogItem>();
            foreach (var item in await GetGachaLogsByTypeAsync(authkey, DDCCPoolType.Beginner, 0))
            {
                res.Add(item.id, ConvertToDDCLLogItem(item, DDCCPoolType.Beginner));
            }
            foreach (var item in await GetGachaLogsByTypeAsync(authkey, DDCCPoolType.Permanent, 0))
            {
                res.Add(item.id, ConvertToDDCLLogItem(item, DDCCPoolType.Permanent));
            }
            foreach (var item in await GetGachaLogsByTypeAsync(authkey, DDCCPoolType.CharacterEvent, 0))
            {
                res.Add(item.id, ConvertToDDCLLogItem(item, DDCCPoolType.CharacterEvent));
            }
            foreach (var item in await GetGachaLogsByTypeAsync(authkey, DDCCPoolType.LCEvent, 0))
            {
                res.Add(item.id, ConvertToDDCLLogItem(item, DDCCPoolType.LCEvent));
            }
            //merger.Merge(res.Values.ToList());
            Console.WriteLine(JsonConvert.SerializeObject(res.Values.ToList(), Formatting.Indented));


        }
    }
}
