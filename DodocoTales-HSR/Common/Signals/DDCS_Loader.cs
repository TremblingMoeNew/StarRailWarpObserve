using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Common.Signals;

namespace DodocoTales.SR.Common.Signals
{
    public delegate void DDCSImportStatusDelegate(DDCCPoolType type, int idx);
}

namespace DodocoTales.SR.Common
{
    public static partial class DDCS
    {
        public static void ExecImportStatusDelegate(DDCSImportStatusDelegate dele, DDCCPoolType type, int idx)
        {
            if (dele != null)
            {
                foreach (DDCSImportStatusDelegate method in dele.GetInvocationList())
                {
                    method.BeginInvoke(type, idx, null, null);
                }
            }
        }

        public static DDCSImportStatusDelegate ImportStatusFromWebRefreshed;
        public static void Emit_ImportStatusFromWebRefreshed(DDCCPoolType type, int current_page)
            => ExecImportStatusDelegate(ImportStatusFromWebRefreshed, type, current_page);

        public static DDCSCommonDelegate ImportConnectionTimeout;
        public static void Emit_ImportConnectionTimeout()
            => ExecCommonDelegate(ImportConnectionTimeout);
        public static DDCSCommonDelegate ImportConnectionRetry;
        public static void Emit_ImportConnectionRetry()
            => ExecCommonDelegate(ImportConnectionRetry);
        public static DDCSCommonDelegate ImportConnectionThrottled;
        public static void Emit_ImportConnectionThrottled()
            => ExecCommonDelegate(ImportConnectionThrottled);

        public static DDCSCommonDelegate ProxyCaptured;
        public static void Emit_ProxyCaptured()
            => ExecCommonDelegate(ProxyCaptured);

    }
}
