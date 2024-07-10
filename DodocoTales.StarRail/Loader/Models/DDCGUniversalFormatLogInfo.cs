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

        [JsonProperty(PropertyName = "lang", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "export_time", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ExportTime { get; set; }

        [JsonIgnore]
        public string ExportTimestamp { get; set; }

        [JsonProperty(PropertyName = "export_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public JToken SerializedTimestamp
        {
            get => int.TryParse(ExportTimestamp, out var ts) ? new JValue(ts) : null;
            set => ExportTimestamp = value?.ToString();
        }

        [JsonProperty(PropertyName = "export_app", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Application { get; set; }

        [JsonProperty(PropertyName = "export_app_version", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ApplicationVersion { get; set; }

        [JsonIgnore]
        public string TimeZone { get; set; }

        [JsonProperty(PropertyName = "region_time_zone", NullValueHandling = NullValueHandling.Ignore)]
        public JToken SerializedTimeZone
        {
            get => int.TryParse(TimeZone, out var tz) ? new JValue(tz) : null;
            set => TimeZone = value?.ToString();
        }

        [JsonProperty(PropertyName = "srgf_version", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string LegacyStandardVersion { get; set; }

        [JsonProperty(PropertyName = "version", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string NewStandardVersion { get; set; }

        [JsonProperty(PropertyName = "game", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Game { get; set; } 

        [JsonProperty(PropertyName = "game_biz", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string GameBiz { get; set; }

        [JsonProperty(PropertyName = "anonymous_export",DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string AnonymousExport { get; set; }


    }
}
