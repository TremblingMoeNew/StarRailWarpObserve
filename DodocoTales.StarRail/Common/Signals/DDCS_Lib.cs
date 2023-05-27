using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodocoTales.SR.Common.Signals;

namespace DodocoTales.SR.Common
{
    public static partial class DDCS
    {
        public static DDCSCommonDelegate BannerLibReloadFailed;
        public static void Emit_BannerLibReloadFailed()
            => ExecCommonDelegate(BannerLibReloadFailed);

        public static DDCSCommonDelegate BannerLibDeserialized;
        public static void Emit_BannerLibDeserialized()
            => ExecCommonDelegate(BannerLibDeserialized);

        public static DDCSCommonDelegate BannerLibReloadCompleted;
        public static void Emit_BannerLibReloadCompleted()
            => ExecCommonDelegate(BannerLibReloadCompleted);


        public static DDCSCommonDelegate UnitLibReloadFailed;
        public static void Emit_UnitLibReloadFailed()
            => ExecCommonDelegate(UnitLibReloadFailed);

        public static DDCSCommonDelegate UnitLibReloadCompleted;
        public static void Emit_UnitLibReloadCompleted()
            => ExecCommonDelegate(UnitLibReloadCompleted);

        public static DDCSUidParamDelegate CurUserSwapping;
        public static void Emit_CurUserSwapping(long uid)
            => ExecUidParamDelegate(CurUserSwapping, uid);

        public static DDCSCommonDelegate CurUserSwapReverted;
        public static void Emit_CurUserSwapReverted()
            => ExecCommonDelegate(CurUserSwapReverted);

        public static DDCSUidParamDelegate CurUserSwapCompleted;
        public static void Emit_CurUserSwapCompleted(long uid)
            => ExecUidParamDelegate(CurUserSwapCompleted, uid);

        public static DDCSCommonDelegate CurUserUpdateCompleted;
        public static void Emit_CurUserUpdateCompleted()
            => ExecCommonDelegate(CurUserUpdateCompleted);

        public static DDCSUidParamDelegate UserLibUidDeplicated;
        public static void Emit_UserLibUidDeplicated(long uid)
            => ExecUidParamDelegate(UserLibUidDeplicated, uid);

        public static DDCSCommonDelegate UserLibReloadCompleted;
        public static void Emit_UserLibReloadCompleted()
            => ExecCommonDelegate(UserLibReloadCompleted);

        public static DDCSCommonDelegate UserlogSaveCompleted;
        public static void Emit_UserlogSaveCompleted()
            => ExecCommonDelegate(UserlogSaveCompleted);

        public static DDCSCommonDelegate UserlogSaveFailed;
        public static void Emit_UserlogSaveFailed()
            => ExecCommonDelegate(UserlogSaveFailed);

        public static DDCSUidParamDelegate V0StyleLogFileDetacted;
        public static void Emit_V0StyleLogFileDetacted(long uid)
            => ExecUidParamDelegate(V0StyleLogFileDetacted, uid);


        public static DDCSCommonDelegate GameClientLibReloadCompleted;
        public static void Emit_GameClientLibReloadCompleted()
            => ExecCommonDelegate(GameClientLibReloadCompleted);

    }
}
