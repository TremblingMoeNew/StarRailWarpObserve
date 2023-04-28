using DodocoTales.SR.Library.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.GameClient.Models
{
    public class DDCLGameClientItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DDCLGameClientType ClientType { get; set; }
        public int TimeZone { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsDefault { get; set; }
    }
}