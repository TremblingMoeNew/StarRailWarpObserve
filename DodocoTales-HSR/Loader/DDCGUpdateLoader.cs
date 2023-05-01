using DodocoTales.SR.Common;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.MetadataLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader
{
    public class DDCGUpdateLoader
    {
        HttpClient client;

        private readonly string ddc_sr_api_meta = "https://sr-api.dodocotales.cc/v1/metadata/";
        private readonly string ddc_sr_api_client = "https://sr-api.dodocotales.cc/v1/download/";
        public string MetadataURL
        {
            get { return ddc_sr_api_meta; }
        }
        public string ClientURL
        {
            get { return ddc_sr_api_client; }
        }

        public readonly string LocalLibPath = "Library/";
        public readonly string BannerLibraryFileName = "BannerLibrary.json";
        public readonly string VersionFileName = "Version.json";
        public readonly string ClientFileName = "Release.zip";

        public DDCGUpdateLoader()
        {
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            client = new HttpClient(handler);
            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            };
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            if(File.Exists(Updater))File.Delete(Updater);
        }


        public async Task<string> GetMetadataFile(string filename)
        {
            string url = MetadataURL + filename;
            try
            {
                return await client.GetStringAsync(url);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<bool> UpdateBannerLibrary()
        {
            try
            {
                var lib = await GetMetadataFile(BannerLibraryFileName);
                var libPath = LocalLibPath + BannerLibraryFileName;
                var stream = File.Open(libPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);
                await writer.WriteAsync(lib);
                await writer.FlushAsync();
                stream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CheckVersion()
        {
            try
            {
                var buffer = await GetMetadataFile(VersionFileName);
                var lib = JsonConvert.DeserializeObject<DDCLMetadataVersion>(buffer);
                if(lib == null)
                {
                    return false;
                }
                var bannerliboldver = DDCL.MetaVersionLib.ConvertLibraryVersionToInt64(DDCL.MetaVersionLib.BannerLibraryVersion);
                var bannerlibnewver = DDCL.MetaVersionLib.ConvertLibraryVersionToInt64(lib.BannerLibraryVersion);
                if (bannerliboldver < bannerlibnewver)
                {
                    if (!await UpdateBannerLibrary())
                    {
                        return false;
                    }
                    DDCL.MetaVersionLib.BannerLibraryVersion = lib.BannerLibraryVersion;
                }
                var clientoldver = DDCL.MetaVersionLib.ConvertClientVersionToInt64(DDCL.MetaVersionLib.ClientVersion);
                var clientnewver = DDCL.MetaVersionLib.ConvertClientVersionToInt64(lib.ClientVersion);

                if (clientoldver < clientnewver)
                {
                    DDCS.Emit_ClientNeedsUpdate();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public static readonly string UpdateStoragePath = "Update/";
        public static readonly string UpdateFilePath = "Update/Latest.zip";
        public static readonly string UpdateExtract = "Update/Latest/";
        public static readonly string Updater = "Updater.exe";


        public async Task DownloadClient()
        {
            var url = ClientURL + ClientFileName ;
            var client = new HttpClient();
            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            };
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            try
            {
                var buf = await client.GetByteArrayAsync(url);
                if (!Directory.Exists(UpdateStoragePath)) Directory.CreateDirectory(UpdateStoragePath);
                File.WriteAllBytes(UpdateFilePath, buf);
            }
            catch (Exception)
            {
                
            }
            DDCS.Emit_ClientUpdateDownloadCompleted();
        }



        public void ApplyUpdate()
        {
            if (File.Exists(UpdateFilePath))
            {
                if (!ExtractClient()) return;
                if (!MoveUpdater()) return;
                ExecUpdater();
            }
        }


        public bool ExtractClient()
        {
            try
            {
                ZipFile.ExtractToDirectory(UpdateFilePath, UpdateExtract);
                //File.Delete(UpdateExtract);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        public bool MoveUpdater()
        {
            if (!File.Exists(UpdateExtract + Updater)) return false;
            try
            {
                if (File.Exists(Updater)) File.Delete(Updater);
                File.Move(UpdateExtract + Updater, Updater);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public void ExecUpdater()
        {
            Process.Start(Updater);
        }
    }
}
