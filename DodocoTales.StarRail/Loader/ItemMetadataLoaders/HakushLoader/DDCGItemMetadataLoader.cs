using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Library.UnitLibrary.Models;
using DodocoTales.SR.Loader.ItemMetadataLoaders.HakushLoader.Enums;
using DodocoTales.SR.Loader.ItemMetadataLoaders.HakushLoader.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader.ItemMetadataLoaders.HakushLoader
{
    internal class DDCGItemMetadataLoader
    {
        HttpClient client;

        public readonly List<string> HakushAPILists = new List<string>
        {
            "https://api.hakush.in/hsr/data/character.json",
            "https://api.hakush.in/hsr/data/lightcone.json",
        };

        public DDCGItemMetadataLoader()
        {
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            client = new HttpClient(handler);
            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            };
            client.DefaultRequestHeaders.CacheControl = cacheControl;
        }

        public async Task<string> GetMetadataFile(string url)
        {
            try
            {
                return await client.GetStringAsync(url);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<DDCLUnitItem>> UpdateUnitLibrary()
        {
            List<DDCLUnitItem> items = new List<DDCLUnitItem>();
            foreach (var url  in HakushAPILists)
            {
                var content = await GetMetadataFile(url);

                if (content == null)
                {
                    return null;
                }

                var res = JsonConvert.DeserializeObject<Dictionary<string, DDCGHakushMetadataItem>>(content);
                if (res == null)
                {
                    continue;
                }

                foreach (var kvp in res)
                {
                    var item = new DDCLUnitItem
                    {
                        ItemID = kvp.Key,
                        Name = kvp.Value.cn,

                    };
                    switch (kvp.Value.rank)
                    {
                        case DDCGHakushMetadataItemTypeRarity.CombatPowerLightconeRarity3:
                        case DDCGHakushMetadataItemTypeRarity.CombatPowerLightconeRarity4:
                        case DDCGHakushMetadataItemTypeRarity.CombatPowerLightconeRarity5:
                            item.UnitType = DDCCUnitType.LightCone; break;
                        case DDCGHakushMetadataItemTypeRarity.CombatPowerAvatarRarityType4:
                        case DDCGHakushMetadataItemTypeRarity.CombatPowerAvatarRarityType5:
                            item.UnitType = DDCCUnitType.Character; break;
                        default:
                            item = null; break;
                    }
                    if (item == null) { continue; }
                    switch (kvp.Value.rank)
                    {
                        case DDCGHakushMetadataItemTypeRarity.CombatPowerLightconeRarity3:
                            item.Rank = 3; break;
                        case DDCGHakushMetadataItemTypeRarity.CombatPowerLightconeRarity4:
                        case DDCGHakushMetadataItemTypeRarity.CombatPowerAvatarRarityType4:
                            item.Rank = 4; break;
                        case DDCGHakushMetadataItemTypeRarity.CombatPowerLightconeRarity5:
                        case DDCGHakushMetadataItemTypeRarity.CombatPowerAvatarRarityType5:
                            item.Rank = 5; break;
                        default:
                            item = null; break;
                    }
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }
        return items;
        }
    }
}
