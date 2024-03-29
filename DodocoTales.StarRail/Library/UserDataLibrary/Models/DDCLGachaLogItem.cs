﻿using DodocoTales.SR.Common.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.UserDataLibrary.Models
{
    public class DDCLGachaLogItem
    {
        public ulong VersionID { get; set; }
        [JsonProperty(PropertyName = "BannerID")]
        public ulong BannerInternalID { get; set; }
        public ulong RoundID { get; set; }

        public ulong ID { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }

        public DDCCPoolType PoolType { get; set; }

        public ulong Rank { get; set; }
        public DDCCUnitType UnitType { get; set; }

        public DDCLGachaLogItemRawData Raw { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Imported;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ImportApplication;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Untrusted;

    }
    public class DDCLGachaLogItemRawData
    {
        public string gacha_id { get; set; }
        public ulong gacha_type { get; set; }
        public string item_id { get; set; }
        public string count { get; set; }
    }
}