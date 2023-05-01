using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Library.UserDataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.CurrentUser.Models
{
    public class DDCLBannerLogItem
    {
        public ulong BannerInternalID { get; set; }

        public ulong VersionID { get; set; }

        public DDCCPoolType CategorizedGachaType { get; set; }

        public List<DDCLRoundLogItem> GreaterRounds { get; set; }
        public List<DDCLGachaLogItem> Logs { get; set; }



    }
}
