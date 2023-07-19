using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Library.GameClient.Models;
using Microsoft.Extensions.FileSystemGlobbing;
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
        readonly string output_log_os = Environment.GetEnvironmentVariable("USERPROFILE") + @"/AppData/LocalLow/Cognosphere/Star Rail/Player.log";
        readonly string game_path_pattern = @"Loading player data from (.+)data.unity3d.+";
        public readonly string StarRailData_CN = "StarRail_Data";
        public readonly string ClientName_CN = "崩坏：星穹铁道";
        public readonly string StarRailData_OS = "StarRail_Data";
        public readonly string ClientName_OS = "Honkai: Star Rail";

        public void LoadGameClientFromGameLog()
        {
            var ls = new List<DDCLGameClientItem>();
            ls.AddRange(LoadGameClientFromGameLogBase(output_log_cn));
            ls.AddRange(LoadGameClientFromGameLogBase(output_log_os));
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
                    };

                    var info = new DirectoryInfo(starrail_data_dir);
                    if (info.Name == StarRailData_CN)
                    {
                        //DDCLog.Info(DCLN.Loader, String.Format("CN client detected: {0}", genshin_data_dir));
                        item.Name = ClientName_CN;
                        item.ClientType = DDCLGameClientType.CN;
                    }
                    else if (info.Name == StarRailData_OS)
                    {
                        item.Name = ClientName_OS;
                        item.ClientType = DDCLGameClientType.Global;
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
                var star_rail_data_dir = exec.DirectoryName + "/" + StarRailData_CN + "/";
                star_rail_data_dir = star_rail_data_dir.Replace('\\', '/');
                if (Directory.Exists(star_rail_data_dir))
                {
                    //DDCLog.Info(DCLN.Loader, String.Format("CN client confirmed: {0}", genshin_data_dir));
                    return new DDCLGameClientItem
                    {
                        Name = null,
                        ClientType = DDCLGameClientType.Unknown,
                        Path = star_rail_data_dir,
                        IsDefault = false
                    };
                }
            }
            return null;
        }


        readonly string WebCachePath = @"./webCaches/**/Cache_Data/data_2";
        // 用CN API强行获取OS或反之，会获得缺失name、item_type与rank_type的记录，造成解析错误
        readonly string authkey_pattern_cn = @"1/0/\S+\?(\S+&game_biz=hkrpg_cn)";
        readonly string authkey_pattern_os = @"1/0/\S+\?(\S+&game_biz=hkrpg_global)";

        public string GetNewestWebCache(string dirpath)
        {
            Matcher matcher = new Matcher();
            matcher.AddInclude(WebCachePath);
            return matcher.GetResultsInFullPath(dirpath).Select(x => new { Path = x, LastModifiedTime = new FileInfo(x).LastWriteTimeUtc })
                .OrderByDescending(x => x.LastModifiedTime).FirstOrDefault()?.Path ?? null;
        }

        public string GetAuthkeyFromWebCache(DDCLGameClientItem client)
        {
            string authkey = null;
            string path = GetNewestWebCache(client.Path);
            if (path == null) return null;
            string authkey_pattern;
            switch (client.ClientType)
            {
                case DDCLGameClientType.CN:
                    authkey_pattern = authkey_pattern_cn;
                    break;
                case DDCLGameClientType.Global:
                    authkey_pattern = authkey_pattern_os;
                    break;
                default:
                    return null;
            }
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
                        Regex regex = new Regex(@"lang=.+?&");
                        authkey = regex.Replace(result[result.Count - 1].Groups[1].Value, "lang=zh-cn&");
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
            string path = GetNewestWebCache(client.Path);
            if (path != null)
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
