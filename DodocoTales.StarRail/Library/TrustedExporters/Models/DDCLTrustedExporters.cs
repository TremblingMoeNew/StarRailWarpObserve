using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.TrustedExporters.Models
{

    public enum DDCLExporterType
    {
        Unknown,
        Trusted,
        Untested,
        Blacklist
    }

    public class DDCLTrustedExporter
    {
        [JsonProperty(PropertyName = "export_app")]
        public string Application { get; set; }

        [JsonProperty(PropertyName = "zh-cn")]
        public string ApplicationNameChinese { get; set; }

        [JsonProperty(PropertyName = "en-us")]
        public string ApplicationNameEnglish { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "opensource")]
        public bool OpenSource { get; set; }

        [JsonProperty(PropertyName = "srgf-partnership", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool SRGFPartnership { get; set; }

        [JsonIgnore]
        public DDCLExporterType ExporterType { get; set; }

    }

    public class DDCLTrustedExporters
    {
        [JsonProperty(PropertyName = "trusted")]
        public List<DDCLTrustedExporter> Trusted { get; set; }

        [JsonProperty(PropertyName = "untested")]
        public List<DDCLTrustedExporter> Untested { get; set; }

        [JsonProperty(PropertyName = "blacklist")]
        public List<DDCLTrustedExporter> BlackList { get; set; }

    }
}
