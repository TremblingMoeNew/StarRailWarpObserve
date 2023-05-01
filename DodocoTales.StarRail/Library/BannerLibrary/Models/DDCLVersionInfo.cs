using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.BannerLibrary.Models
{
    public class DDCLVersionInfo
    {
        public ulong ID { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<DDCLBannerInfo> Banners { get; set; }
    }
}
