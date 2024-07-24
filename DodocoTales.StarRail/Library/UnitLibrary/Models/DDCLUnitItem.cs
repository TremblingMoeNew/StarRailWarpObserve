using DodocoTales.SR.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.UnitLibrary.Models
{
    public class DDCLUnitItem
    {
        public string ItemID { get; set; }
        public string Name { get; set; }
        public ulong Rank { get; set; }
        public DDCCUnitType UnitType { get; set; }
    }
}
