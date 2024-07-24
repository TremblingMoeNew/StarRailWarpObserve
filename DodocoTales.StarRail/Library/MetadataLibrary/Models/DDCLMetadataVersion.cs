using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.MetadataLibrary.Models
{
    public class DDCLMetadataVersion
    {
        public string ClientVersion { get; set; }
        public string BannerLibraryVersion { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ExportersLibraryVersion { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public DateTime UnitLibLastUpdateAt { get; set; }
    }
}
