using DodocoTales.SR.Library.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.UserDataLibrary.Models
{
    public class DDCLUserGachaLog
    {
        public long UID { get; set; }
        public int TimeZone { get; set; }

        public DDCLGameClientType ClientType { get; set; }

        public List<DDCLGachaLogItem> LogsV1 { get; set; }

        [JsonIgnore]
        public List<DDCLGachaLogItem> Logs
        {
            get { return LogsV1; }
            set { LogsV1 = value; }
        }

    }
}
