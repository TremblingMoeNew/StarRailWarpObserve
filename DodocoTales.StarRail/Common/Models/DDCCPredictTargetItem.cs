using DodocoTales.SR.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Common.Models
{
    public class DDCCPredictTargetItem
    {
        public DDCCPoolType PoolType { get; set; }
        public int PreviousCount { get; set; }
        public bool IsPreviousPermanant { get; set; }
    }
}
