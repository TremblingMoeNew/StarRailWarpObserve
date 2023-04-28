using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader
{
    public static partial class DDCG
    {
        public static DDCGGameClientLoader GameClientLoader = new DDCGGameClientLoader();
        public static DDCGProxyLoader ProxyLoader = new DDCGProxyLoader();

        public static DDCGWebGachaLogLoader WebLogLoader = new DDCGWebGachaLogLoader();
    }
}
