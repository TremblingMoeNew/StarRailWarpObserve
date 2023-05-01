using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.BannerLibrary.Models
{
    public class DDCLBannerLibModel
    {
        [JsonProperty(PropertyName = "Beginner")]
        public List<DDCLBannerInfo> BeginnerPools { get; set; }
        [JsonProperty(PropertyName = "Permanent")]
        public List<DDCLBannerInfo> PermanentPools { get; set; }

        [JsonProperty(PropertyName = "Event")]
        public List<DDCLVersionInfo> EventPools { get; set; }
    }
}
