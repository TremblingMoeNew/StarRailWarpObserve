using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader.Models
{
    public class DDCGUniversalFormatLogItem
    {
        public string gacha_id { get; set; }
        public string gacha_type { get; set; }
        public string item_id { get; set; }
        public string count { get; set; }
        public string time { get; set; }
        public string name { get; set; }
        public string item_type { get; set; }
        public string rank_type { get; set; }
        public string id { get; set; }
        [JsonProperty(DefaultValueHandling=DefaultValueHandling.IgnoreAndPopulate)]
        public string __hash { get; set; }
    }
}
