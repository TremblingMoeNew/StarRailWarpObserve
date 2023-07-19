using DodocoTales.SR.Common;
using DodocoTales.SR.Gui.Models;
using DodocoTales.SR.Library.UserDataLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.UserDataLibrary
{
    public class DDCLUserDataLibrary
    {
        public readonly string UserDataDirPath = "userdata";
        public readonly string UserDataFileSearchPattern = "userlog_*.json";
        public readonly string UserDataFileRegexPattern = @"userlog_(\d+)\.json";
        public readonly string UserDataFileOpenPattern = "userdata/userlog_{0}.json";
        public readonly string UserBackupFileOpenPattern = "userdata/backup_{0}_{1:yyyyMMddhhmmss}.json";
        public Dictionary<long, DDCLUserGachaLog> U { get; set; }

        public DDCLUserDataLibrary()
        {
            U = new Dictionary<long, DDCLUserGachaLog>();
        }
        public DDCLUserGachaLog CreateEmptyLocalGachaLog(long uid)
        {
            return new DDCLUserGachaLog { UID = uid, Logs = new List<DDCLGachaLogItem>() };
        }
        public void TryAddEmptyUser(long uid)
        {
            if (U.ContainsKey(uid)) return;
            var log = CreateEmptyLocalGachaLog(uid);
            U.Add(uid, log);
            //DDCLog.Info(DCLN.Lib, String.Format("New user added. UID:{0}", uid));
        }
        public async Task LoadLocalGachaLogByUidAsync(long uid)
        {
            string logfile = String.Format(UserDataFileOpenPattern, uid);
            //DDCLog.Info(DCLN.Lib, String.Format("Loading userlog file: {0}", logfile));
            try
            {
                var stream = File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(stream);
                var response = JsonConvert.DeserializeObject<DDCLUserGachaLog>(await reader.ReadToEndAsync());
                if (U.ContainsKey(response.UID))
                {
                    DDCS.Emit_UserLibUidDeplicated(response.UID);
                    //DDCLog.Warning(DCLN.Lib, String.Format("Userlog deplicated. UID:{0}", response.uid));
                    return;
                }
                if (uid != response.UID)
                {
                    //DDCLog.Warning(DCLN.Lib, String.Format("{0}, UID:{1}", logfile, response.uid));
                }
                U[response.UID] = response;
                //DDCLog.Info(DCLN.Lib, String.Format("Userlog successfully loaded. UID:{0}", response.uid));

            }
            catch (Exception e)
            {
                //DDCLog.Error(DCLN.Lib, String.Format("Failed to load userlog. UID:{0}", uid), e);
            }
        }
        public async Task LoadLocalGachaLogsAsync()
        {
            //DDCLog.Info(DCLN.Lib, "Loading userdata...");
            DirectoryInfo dir = new DirectoryInfo(UserDataDirPath);
            if (!dir.Exists) dir.Create();
            var files = dir.GetFiles(UserDataFileSearchPattern);
            List<Task> taskQuery = new List<Task>();
            foreach (var f in files)
            {
                var result = Regex.Match(f.Name, UserDataFileRegexPattern);
                long uid = 0;
                try
                {
                    uid = Convert.ToInt64(result.Groups[1].Value);
                }
                catch (Exception e)
                {
                }
                taskQuery.Add(LoadLocalGachaLogByUidAsync(uid));
            }
            await Task.WhenAll(taskQuery);
            DDCS.Emit_UserLibReloadCompleted();
            //DDCLog.Info(DCLN.Lib, "Userdata successfully loaded.");
        }
        public async Task SaveUserAsync(DDCLUserGachaLog userlog)
        {
            if (userlog == null) return;
            string logfile = String.Format(UserDataFileOpenPattern, userlog.UID);
            // TODO: LibVersion Update
            try
            {
                var stream = File.Open(logfile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);
                var serialized = JsonConvert.SerializeObject(userlog, Formatting.Indented);
                await writer.WriteAsync(serialized);
                await writer.FlushAsync();
                stream.Close();
                DDCS.Emit_UserlogSaveCompleted();
                //DDCLog.Info(DCLN.Lib, String.Format("Userlog successfully saved. UID:{0}", userlog.uid));
            }
            catch (Exception e)
            {
                DDCS.Emit_UserlogSaveFailed();
                //DDCLog.Error(DCLN.Lib, String.Format("Failed to save userlog. UID:{0}", userlog.uid), e);
            }
        }

        public async Task BackupUserAsync(DDCLUserGachaLog userlog)
        {
            if (userlog == null) return;
            string logfile = String.Format(UserBackupFileOpenPattern, userlog.UID, DateTime.Now);
            try
            {
                var stream = File.Open(logfile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);
                var serialized = JsonConvert.SerializeObject(userlog, Formatting.Indented);
                await writer.WriteAsync(serialized);
                await writer.FlushAsync();
                stream.Close();
                DDCS.Emit_UserlogBackupCompleted();
                //DDCLog.Info(DCLN.Lib, String.Format("Userlog successfully saved. UID:{0}", userlog.uid));
            }
            catch (Exception e)
            {
                DDCS.Emit_UserlogBackupFailed();
                //DDCLog.Error(DCLN.Lib, String.Format("Failed to save userlog. UID:{0}", userlog.uid), e);
            }
        }

        public DDCLUserGachaLog GetUserLogByUid(long uid)
        {
            TryAddEmptyUser(uid);
            return U[uid];
        }

        public bool UserExists(long uid)
        {
            return U.ContainsKey(uid);
        }

        public async Task RemoveUserByUid(long uid)
        {
            if (!U.ContainsKey(uid)) return;
            await BackupUserAsync(GetUserLogByUid(uid));
            try
            {
                File.Delete(String.Format(UserDataFileOpenPattern, uid));
            }
            catch 
            {
                DDCS.Emit_UserlogRemoveFailed();
                return;
            }
            U.Remove(uid);
            DDCS.Emit_UserlogRemoveCompleted();

        }
    }
}
