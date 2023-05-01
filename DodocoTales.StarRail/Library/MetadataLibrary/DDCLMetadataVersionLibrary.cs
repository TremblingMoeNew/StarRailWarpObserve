using DodocoTales.SR.Library.MetadataLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Diagnostics;

namespace DodocoTales.SR.Library.MetadataLibrary
{
    public class DDCLMetadataVersionLibrary
    {
        public readonly string libPath = @"Library/Version.json";
        public readonly string libUpdateInfoPath = @"Library/Version.Update.json";
        DDCLMetadataVersion model;
        public string ClientVersion { get { return model?.ClientVersion; } set { if (model == null) model = new DDCLMetadataVersion(); model.ClientVersion = value; SaveLibrary(); } }
        public string BannerLibraryVersion { get { return model?.BannerLibraryVersion; } set { if (model == null) model = new DDCLMetadataVersion(); model.BannerLibraryVersion = value; SaveLibrary(); } }

        public bool FirstRunAfterUpdate { get; set; }

        public async Task Initialize()
        {
            await LoadLibraryAsync();
            if(await CheckVersionUpdateFileAsync())
            {
                FirstRunAfterUpdate = true;
            }
        }


        public async Task<bool> CheckVersionUpdateFileAsync()
        {
            if (File.Exists(libUpdateInfoPath))
            {
                DDCLMetadataVersion updatemodel;
                try
                {
                    var stream = File.Open(libUpdateInfoPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader reader = new StreamReader(stream);
                    var buffer = await reader.ReadToEndAsync();
                    stream.Close();
                    File.Delete(libUpdateInfoPath);
                    updatemodel = JsonConvert.DeserializeObject<DDCLMetadataVersion>(buffer);
                }
                catch (Exception e)
                {
                    return false;
                }
                if (model == null)
                {
                    model = updatemodel;
                    SaveLibrary();
                }
                else if (updatemodel.ClientVersion != null)
                {
                    ClientVersion = updatemodel.ClientVersion;  
                }
                return true;
            }
            return false;
        }



        public async Task<bool> LoadLibraryAsync()
        {
            if (!Directory.Exists("Library"))
            {
                Directory.CreateDirectory("Library");
                model = null;
                return false;
            }
            try
            {
                var stream = File.Open(libPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(stream);
                var buffer = await reader.ReadToEndAsync();
                stream.Close();
                model = JsonConvert.DeserializeObject<DDCLMetadataVersion>(buffer);
            }
            catch (Exception)
            {
                model = null;
                return false;
            }
            return true;
        }

        public bool SaveLibrary()
        {
            try
            {
                var stream = File.Open(libPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);
                var serialized = JsonConvert.SerializeObject(model, Formatting.Indented);
                writer.Write(serialized);
                writer.Flush();
                stream.Close();
            }
            catch (Exception)
            {
                /// TODO: 报告保存错误？
                return false;
            }
            return true;
        }

        public long ConvertClientVersionToInt64(string clientver)
        {
            if (clientver == null) return 0;
            var res = Regex.Match(clientver, @"^.*(\d+)\.(\d+)\.(\d+)\.{0,1}(\d*).*");
            if (res.Groups.Count < 4) return 0;
            long intver = 0;
            intver += Convert.ToInt32(res.Groups[1].Value);
            intver = intver * 100 + Convert.ToInt32(res.Groups[2].Value);
            intver = intver * 100 + Convert.ToInt32(res.Groups[3].Value);
            intver *= 1000000;
            var last = res.Groups[4].Value;
            if (last != "")
            {
                intver += Convert.ToInt32(last);
            }
            return intver;
        }
        public long ConvertLibraryVersionToInt64(string libver)
        {
            if (libver == null) return 0;
            var res = Regex.Match(libver, @"^.*(\d+)\.(\d+)\.(\d+)-.*(\d+)\.(\d+)\.(\d+)-(\d+).*");
            if (res.Groups.Count < 7) return 0;
            long intver = 0;
            for (int i = 1; i < 7; i++)
            {
                intver = intver * 100 + Convert.ToInt32(res.Groups[i].Value);
            }
            intver = intver * 1000000 + Convert.ToInt32(res.Groups[7].Value);
            return intver;
        }

    }
}
