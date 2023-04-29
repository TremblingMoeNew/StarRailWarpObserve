using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Library.BannerLibrary;
using DodocoTales.SR.Library.CurrentUser;
using DodocoTales.SR.Library.GameClient;
using DodocoTales.SR.Library.UserDataLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library
{
    public static class DDCL
    {
        public static DDCLUserDataLibrary UserDataLib = new DDCLUserDataLibrary();
        public static DDCLBannerLibrary BannerLib = new DDCLBannerLibrary();

        public static DDCLCurrentUserLibrary CurrentUser = new DDCLCurrentUserLibrary();

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

        public static readonly int DefaultTimeZone = 8;
        public static DateTimeOffset GetTimeOffset(DateTime time, int zone)
        {
            return new DateTimeOffset(time, GetZoneOffsetTimeSpan(zone));
        }
        public static DateTimeOffset GetSyncTimeOffset(DateTime time)
        {
            return new DateTimeOffset(time, GetZoneOffsetTimeSpan(DDCL.DefaultTimeZone));
        }
        public static DateTimeOffset GetNowDateTimeOffset()
        {
            return new DateTimeOffset(DateTime.Now);
        }
        public static int CheckTimeIsBetween(DateTimeOffset begin, DateTimeOffset end, DateTimeOffset time)
        {
            if (DateTimeOffset.Compare(end, time) < 0) return -1;
            else if (DateTimeOffset.Compare(begin, time) > 0) return 1;
            else return 0;
        }
        public static DateTimeOffset GetBannerTimeOffset(DateTime time, bool sync, int zone)
        {
            if (sync) return GetSyncTimeOffset(time);
            else return GetTimeOffset(time, zone);
        }
        public static TimeSpan GetZoneOffsetTimeSpan(int zone)
        {
            return new TimeSpan(zone, 0, 0);
        }
    }
}
