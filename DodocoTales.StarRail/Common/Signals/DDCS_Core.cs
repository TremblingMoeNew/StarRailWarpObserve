using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodocoTales.SR.Common.Signals;

namespace DodocoTales.SR.Common.Signals
{
    public delegate void DDCSCommonDelegate();
    public delegate void DDCSUidParamDelegate(long uid);
}

namespace DodocoTales.SR.Common
{
    public static partial class DDCS
    {
        public static void ExecCommonDelegate(DDCSCommonDelegate dele)
        {
            if (dele != null)
            {
                foreach (DDCSCommonDelegate method in dele.GetInvocationList())
                {
                    method.BeginInvoke(null, null);
                }
            }
        }
        public static void ExecUidParamDelegate(DDCSUidParamDelegate dele, long uid)
        {
            if (dele != null)
            {
                foreach (DDCSUidParamDelegate method in dele.GetInvocationList())
                {
                    method.BeginInvoke(uid, null, null);
                }
            }
        }
    }
}
