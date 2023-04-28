using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Library.GameClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library
{
    public static class DDCL
    {
        public static DDCLGameClientLibrary GameClientLib = new DDCLGameClientLibrary();



        public static DDCCUnitType ConvertToUnitType(string typename)
        {
            switch (typename)
            {
                case "角色":
                    return DDCCUnitType.Character;
                case "光锥":
                    return DDCCUnitType.LightCone;
                default:
                    return DDCCUnitType.Unknown;
            }
        }
    }
}
