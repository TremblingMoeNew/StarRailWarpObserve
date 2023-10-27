using DodocoTales.SR.Library.TrustedExporters.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.TrustedExporters
{
    public class DDCLTrustedExportersLibrary
    {
        public readonly string libPath = @"Library/Exporters.json";
        DDCLTrustedExporters model;
        List<DDCLTrustedExporter> exporters = new List<DDCLTrustedExporter>();

        public async Task<bool> LoadLibraryAsync()
        {
            exporters.Clear();
            try
            {
                var stream = File.Open(libPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(stream);
                var buffer = await reader.ReadToEndAsync();
                stream.Close();
                model = JsonConvert.DeserializeObject<DDCLTrustedExporters>(buffer);
            }
            catch (Exception)
            {
                model = null;
                return false;
            }
            model.Trusted.ForEach(x => { x.ExporterType = DDCLExporterType.Trusted; });
            model.BlackList.ForEach(x => { x.ExporterType = DDCLExporterType.Blacklist; });
            model.Untested.ForEach(x => { x.ExporterType = DDCLExporterType.Untested; });
            exporters.AddRange(model.Trusted);
            exporters.AddRange(model.BlackList);
            exporters.AddRange(model.Untested);

            return true;
        }

        public DDCLTrustedExporter GetExporter(string export_app)
        {
            var res = exporters.FirstOrDefault(x=>x.Application == export_app);
            if (res == null)
            {
                res = new DDCLTrustedExporter
                {
                    Application = export_app,
                    ApplicationNameChinese = export_app,
                    ApplicationNameEnglish = export_app,
                    Author = "Unknown",
                    ExporterType = DDCLExporterType.Unknown,
                    OpenSource = true
                };
            }
            return res;
        }
    }
}
