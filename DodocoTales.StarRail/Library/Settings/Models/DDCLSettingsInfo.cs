using DodocoTales.SR.Library.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.Settings.Models
{
    public class DDCLSettingsInfo
    {
        public long LastUserUID { get; set; }

        [DefaultValue(DDCLUnitMetadataSource.Hakush)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public DDCLUnitMetadataSource MetadataSource { get; set; }
    }
}
