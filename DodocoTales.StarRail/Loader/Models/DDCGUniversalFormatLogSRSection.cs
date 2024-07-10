using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader.Models
{
    public class DDCGUniversalFormatLogSRSection
    {
        [JsonProperty(PropertyName = "uid", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string UID { get; set; }

        [JsonProperty(PropertyName = "lang", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Language { get; set; } = "zh-cn";

        [JsonIgnore]
        public string TimeZone { get; set; }

        [JsonProperty(PropertyName = "timezone")]
        public JToken SerializedTimeZone
        {
            get => int.TryParse(TimeZone, out var tz) ? new JValue(tz) : null;
            set => TimeZone = value?.ToString();
        }

        [JsonProperty(PropertyName = "list")]
        public List<DDCGUniversalFormatLogItem> List { get; set; }



        // Non-standard Fields

        [JsonProperty(PropertyName = "game_biz", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string GameBiz { get; set; }

    }
}
