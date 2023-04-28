using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader.Models
{
    public class DDCGGachaLogResponseItem
    {
        public long uid { get; set; }
        public string gacha_id { get; set; }
        public ulong gacha_type { get; set; }
        public string item_id { get; set; }
        public string count { get; set; }
        public DateTime time { get; set; }
        public string name { get; set; }
        public string lang { get; set; }
        public string item_type { get; set; }
        public ulong rank_type { get; set; }
        public ulong id { get; set; }
    }
}
