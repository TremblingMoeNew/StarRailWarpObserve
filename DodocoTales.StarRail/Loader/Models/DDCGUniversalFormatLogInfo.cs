using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader.Models
{
    public class DDCGUniversalFormatLogInfo
    {
        [JsonProperty(PropertyName = "uid",DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string UID { get; set; }

        [JsonProperty(PropertyName = "lang")]
        public string Language { get; set; } = "zh-cn";

        [JsonProperty(PropertyName = "export_time")]
        public string ExportTime { get; set; }

        [JsonIgnore]
        public string ExportTimestamp { get; set; }

        [JsonProperty(PropertyName = "export_timestamp")]
        public JToken SerializedTimestamp
        {
            get => int.TryParse(ExportTimestamp, out var ts) ? new JValue(ts) : null;
            set => ExportTimestamp = value.ToString();
        }

        [JsonProperty(PropertyName = "export_app")]
        public string Application { get; set; }

        [JsonProperty(PropertyName = "export_app_version")]
        public string ApplicationVersion { get; set; }

        [JsonIgnore]
        public string TimeZone { get; set; }

        [JsonProperty(PropertyName = "region_time_zone")]
        public JToken SerializedTimeZone
        {
            get => int.TryParse(TimeZone,out var tz) ? new JValue(tz) : null;
            set => TimeZone = value.ToString();
        }

        [JsonProperty(PropertyName = "srgf_version")]
        public string StandardVersion { get; set; } 

        [JsonProperty(PropertyName = "game")]
        public string Game { get; set; } 

        [JsonProperty(PropertyName = "game_biz")]
        public string GameBiz { get; set; }

        [JsonProperty(PropertyName = "anonymous_export",DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string AnonymousExport { get; set; }
    }
}
