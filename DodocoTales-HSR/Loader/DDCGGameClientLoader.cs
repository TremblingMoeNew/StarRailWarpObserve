using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Library.GameClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader
{
    public class DDCGGameClientLoader
    {
        readonly string output_log_cn = Environment.GetEnvironmentVariable("USERPROFILE") + @"/AppData/LocalLow/miHoYo/崩坏：星穹铁道/Player.log";
        readonly string game_path_pattern = @"Loading player data from (.+)data.unity3d.+";
        public readonly string StarRailData_CN = "StarRail_Data";
        public readonly string ClientName_CN = "崩坏：星穹铁道";

        public void LoadGameClientFromGameLog()
        {
            var ls = new List<DDCLGameClientItem>();
            ls.AddRange(LoadGameClientFromGameLogBase(output_log_cn));
            var exists = DDCL.GameClientLib.GetClients().Select(x => x.Path);
            ls.RemoveAll(x => exists.Contains(x.Path));
            DDCL.GameClientLib.AddClients(ls);
        }

        private List<DDCLGameClientItem> LoadGameClientFromGameLogBase(string output_log)
        {
            var items = new List<DDCLGameClientItem>();
            try
            {
                using (var stream = File.Open(output_log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    StreamReader reader = new StreamReader(stream);
                    var log = reader.ReadToEnd();
                    var result = Regex.Match(log, game_path_pattern);
                    var starrail_data_dir = result.Groups[1].Value;

                    var item = new DDCLGameClientItem
                    {
                        Path = starrail_data_dir,
                        IsDefault = false,
                        TimeZone = 8
                    };

                    var info = new DirectoryInfo(starrail_data_dir);
                    if (info.Name == StarRailData_CN)
                    {
                        //DDCLog.Info(DCLN.Loader, String.Format("CN client detected: {0}", genshin_data_dir));
                        item.Name = ClientName_CN;
                        item.ClientType = DDCLGameClientType.CN;
                    }
                    else
                    {
                        item = null;
                    }
                    if (item != null) items.Add(item);
                }
            }
            catch
            {

            }
            return items;
        }


        public readonly string ClientExecutable = "StarRail.exe";

        public DDCLGameClientItem LoadGameClientItemFromExecutablePath(string execpath)
        {
            FileInfo exec = new FileInfo(execpath);
            if (exec.Name == ClientExecutable)
            {
                var genshin_data_dir = exec.DirectoryName + "/" + StarRailData_CN + "/";
                genshin_data_dir = genshin_data_dir.Replace('\\', '/');
                if (Directory.Exists(genshin_data_dir))
                {
                    //DDCLog.Info(DCLN.Loader, String.Format("CN client confirmed: {0}", genshin_data_dir));
                    return new DDCLGameClientItem
                    {
                        Name = ClientName_CN,
                        ClientType = DDCLGameClientType.CN,
                        Path = genshin_data_dir,
                        TimeZone = 8,
                        IsDefault = false
                    };
                }
            }
            return null;
        }


        readonly string WebCachePath = @"/webCaches/Cache/Cache_Data/data_2";
        readonly string authkey_pattern = @"1/0/\S+\?(\S+&game_biz=hkrpg_cn)";

        public string GetAuthkeyFromWebCache(DDCLGameClientItem client)
        {
            string authkey = null;
            string path = client.Path + WebCachePath;
            if (!File.Exists(path))
            {
                return null;
            }
            string target = Path.GetTempFileName();
            File.Copy(path, target, true);
            try
            {
                using (var stream = File.Open(target, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StreamReader reader = new StreamReader(stream);
                    var log = reader.ReadToEnd();
                    var result = Regex.Matches(log, authkey_pattern);
                    if (result.Count > 0)
                    {
                        Regex regex = new Regex(@"lang=.+&authkey=");
                        authkey = regex.Replace(result[result.Count - 1].Groups[1].Value, "lang=en-us&authkey=");
                    }
                }
            }
            catch (Exception ex)
            {
                authkey = null;
                Console.WriteLine(ex.Message);
            }
            File.Delete(target);
            return authkey;
        }

        public void RemoveCacheFile(DDCLGameClientItem client)
        {
            string path = client.Path + WebCachePath;
            try
            {
                File.Delete(path);
                //DDCLog.Info(DCLN.Loader, String.Format("Remove cache success: {0}", path));
            }
            catch
            {
                //DDCLog.Warning(DCLN.Loader, String.Format("Remove cache failed: {0}", path));
            }

        }
    }
}
