using DodocoTales.SR.Common;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.MetadataLibrary.Models;
using DodocoTales.SR.Loader.Models;
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
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.Http;

namespace DodocoTales.SR.Loader
{
    public class DDCGUpdateLoader
    {
        HttpClient client;

        private readonly string ddc_sr_api_meta = "https://sr-api.dodocotales.cc/v1/metadata/";
        private readonly string ddc_sr_api_client = "https://sr-api.dodocotales.cc/v1/download/";
        private readonly string starwo_changelog = "https://starwo.dodocotales.cc/changelog.md";
        public string MetadataURL
        {
            get { return ddc_sr_api_meta; }
        }
        public string ClientURL
        {
            get { return ddc_sr_api_client; }
        }

        public string ChangeLogURL
        {
            get => starwo_changelog;
        }
        public readonly string LocalLibPath = "Library/";
        public readonly string BannerLibraryFileName = "BannerLibrary.json";
        public readonly string ExportersFileName = "Exporters.json";
        public readonly string VersionFileName = "Version.json";
        public readonly string ClientFileName = "Release.zip";

        public readonly string DependencyURLPath = "Dependencies/SkiaSharp-Native/";


        public string ChangeLog { get; set; }   

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

        public async Task<bool> UpdateExportersLibrary()
        {
            try
            {
                var lib = await GetMetadataFile(ExportersFileName);
                var libPath = LocalLibPath + ExportersFileName;
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

                var exporterliboldver = DDCL.MetaVersionLib.ConvertLibraryVersionToInt64(DDCL.MetaVersionLib.ExportersLibraryVersion);
                var exporterlibnewver = DDCL.MetaVersionLib.ConvertLibraryVersionToInt64(lib.ExportersLibraryVersion);
                if (exporterliboldver < exporterlibnewver)
                {
                    if (!await UpdateExportersLibrary())
                    {
                        return false;
                    }
                    DDCL.MetaVersionLib.ExportersLibraryVersion = lib.ExportersLibraryVersion;
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

        public async Task LoadChangeLog()
        {
            try
            {
                ChangeLog = await client.GetStringAsync(ChangeLogURL);
                Console.WriteLine(ChangeLog);
            }
            catch
            {
                ChangeLog = null;
            }
        }

        public static readonly string UpdateStoragePath = "Update/";
        public static readonly string UpdateFileName = "Latest.zip";
        public static readonly string UpdateExtract = "Update/Latest/";
        public static readonly string Updater = "Updater.exe";


        public async Task DownloadClient()
        {
            if (await DownloadZip(new DDCGDownloadTask { TaskType = DDCGDownloadTaskType.ClientUpdate, LocalFileName = UpdateFileName, RemoteFileName = ClientFileName }))
                DDCS.Emit_ClientUpdateDownloadCompleted();
            else
                DDCS.Emit_ClientUpdateDownloadFailed();
        }


        public bool DependencyExist(string arch)
        {
            return Directory.Exists(arch) && File.Exists($"{arch}/libSkiaSharp.dll") && File.Exists($"{arch}/libHarfBuzzSharp.dll");
        }


        public async Task DownloadDependency(string arch)
        {
            if(await DownloadZip(new DDCGDownloadTask { TaskType = DDCGDownloadTaskType.DependencyUpdate, LocalFileName = $"{arch}.zip", RemoteFileName = $"{arch}.zip" }))
            {
                if (ApplyDependency(arch))
                {
                    DDCS.Emit_DependencyUpdateDownloadCompleted();
                    return;
                }
            }
            DDCS.Emit_DependencyUpdateDownloadFailed();

        }



        public async Task<bool> DownloadZip(DDCGDownloadTask task)
        {
            var url = task.TaskType == DDCGDownloadTaskType.ClientUpdate?
                ClientURL + task.RemoteFileName : ClientURL + DependencyURLPath + task.RemoteFileName;

            var client = new HttpClient();
            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            };
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            client.Timeout = TimeSpan.FromMinutes(10);
            var ms = new MemoryStream();
            try
            {
                Console.WriteLine(url);
                string md5_expected="";
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    md5_expected= response.Headers.ETag.ToString().Trim('"');
                    Console.WriteLine(md5_expected);
                    task.FileSize = response.Content.Headers.ContentLength;
                    task.DownloadedSize = 0;
                    task.EstSpeed = 0;
                    Console.WriteLine($"Size: {task.FileSize}");
                    int length;
                    var buffer = new byte[1 << 20];
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var lastTimePoint = DateTime.Now;
                        var lengthLTP2Now = 0;
                        DDCS.Emit_ClientUpdateDownloadStatusReport(task);
                        while ((length = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
                        {
                            ms.Write(buffer, 0, length);
                            task.DownloadedSize += length;
                            lengthLTP2Now += length;
                            var currentTimePoint = DateTime.Now;
                            var timedelta = (currentTimePoint - lastTimePoint).TotalSeconds;
                            if(timedelta > 1)
                            {
                                lastTimePoint = currentTimePoint;
                                task.EstSpeed = lengthLTP2Now / timedelta;
                                Console.WriteLine($"Download Progress: {task.DownloadedSize / 1024.0:F2}KB/{task.FileSize / 1024.0:F2}KB, Speed:{task.EstSpeed/1024:F2}KB/s, EST: {(task.FileSize - task.DownloadedSize) / (lengthLTP2Now / timedelta):F2}s");
                                lengthLTP2Now = 0;
                                DDCS.Emit_ClientUpdateDownloadStatusReport(task);
                            }
                        }
                    }

                }
                var buf = ms.ToArray();
                var md5_actual = BitConverter.ToString(MD5.Create().ComputeHash(buf)).Replace("-","").ToLower();
                
                Console.WriteLine($"Size: {buf.Length}, MD5: {md5_actual}, {md5_expected == md5_actual}");
                if(md5_expected == md5_actual)
                {
                    if (!Directory.Exists(UpdateStoragePath)) Directory.CreateDirectory(UpdateStoragePath);
                    File.WriteAllBytes(UpdateStoragePath + task.LocalFileName, buf);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        public bool ApplyDependency(string arch)
        {
            try
            {
                ZipFile.ExtractToDirectory($"{UpdateStoragePath}/{arch}.zip", $"{UpdateStoragePath}/{arch}/");
                File.Delete($"{UpdateStoragePath}/{arch}.zip");

                if (!Directory.Exists(arch)) Directory.CreateDirectory(arch);
                var destDir = new DirectoryInfo(arch);
                var srcDir = new DirectoryInfo($"update/{arch}/");
                foreach (var file in srcDir.GetFiles())
                {
                    var destl = destDir.GetFiles(file.Name);
                    if (destl.Any())
                    {
                        destl[0].Delete();
                    }
                    file.MoveTo(destDir.FullName + "/" + file.Name);
                }
                srcDir.Delete();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }



        public void ApplyUpdate()
        {
            if (File.Exists(UpdateStoragePath + UpdateFileName))
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
                ZipFile.ExtractToDirectory(UpdateStoragePath + UpdateFileName, UpdateExtract);
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
