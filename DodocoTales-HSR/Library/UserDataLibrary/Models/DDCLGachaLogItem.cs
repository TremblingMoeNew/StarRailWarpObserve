using DodocoTales.SR.Common.Enums;
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
        public ulong version_id { get; set; }
        public ulong banner_id { get; set; }
        public ulong round_id { get; set; }
        public ulong internal_id { get; set; }

        public ulong id { get; set; }
        public string name { get; set; }
        public DateTime time { get; set; }

        public DDCCPoolType pooltype { get; set; }

        public ulong rank { get; set; }
        public DDCCUnitType unittype { get; set; }

        public DDCLGachaLogItemRawData raw { get; set; }

    }
    public class DDCLGachaLogItemRawData
    {
        public string gacha_id { get; set; }
        public ulong gacha_type { get; set; }
        public string item_id { get; set; }
        public string count { get; set; }
    }
}