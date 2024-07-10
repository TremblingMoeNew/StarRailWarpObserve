using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader.Models
{
    public class DDCGUniversalFormatLog
    {
        [JsonProperty(PropertyName = "info")]
        public DDCGUniversalFormatLogInfo Info { get; set; }

        [JsonProperty(PropertyName = "list", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public List<DDCGUniversalFormatLogItem> List { get; set; }

        [JsonProperty(PropertyName = "hkrpg", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public List<DDCGUniversalFormatLogSRSection> StarRailSections { get; set; }
    }
}
