using Newtonsoft.Json;
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

        [JsonProperty(PropertyName = "export_app")]
        public string Application { get; set; } = "DodocoTales.StarRail";

        [JsonProperty(PropertyName = "export_app_version")]
        public string ApplicationVersion { get; set; }

        [JsonProperty(PropertyName = "game")]
        public string Game { get; set; } = "Honkai_Star_Rail";

        [JsonProperty(PropertyName = "region_time_zone")]
        public string TimeZone { get; set; }

        [JsonProperty(PropertyName = "game_biz")]
        public string GameBiz { get; set; }

        [JsonProperty(PropertyName = "anonymous_export",DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string AnonymousExport { get; set; }
    }
}
