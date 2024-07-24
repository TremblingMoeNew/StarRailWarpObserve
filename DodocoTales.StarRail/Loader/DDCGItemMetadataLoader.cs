using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Library.UnitLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DodocoTales.SR.Loader
{
    public class DDCGItemMetadataLoader
    {
        readonly ItemMetadataLoaders.HakushLoader.DDCGItemMetadataLoader HakushLoader = new ItemMetadataLoaders.HakushLoader.DDCGItemMetadataLoader();

        public readonly string LocalLibPath = "Library/";
        public readonly string UnitLibraryFileName = "UnitLibrary.json";

        public async Task<bool> UpdateUnitLibrary()
        {
            List<DDCLUnitItem> items = null;
            switch (DDCL.SettingsLib.MetadataSource)
            {
                case DDCLUnitMetadataSource.Hakush:
                    items = await HakushLoader.UpdateUnitLibrary();
                    break;
                default:
                    return true ;
            }
            if (items == null)
            {
                return false;
            }

            var lib = JsonConvert.SerializeObject(items, Formatting.Indented);

            if (lib == null)
            {
                return false;
            }
            try
            {
                var libPath = LocalLibPath + UnitLibraryFileName;
                var stream = File.Open(libPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);
                await writer.WriteAsync(lib);
                await writer.FlushAsync();
                stream.Close();

                DDCL.MetaVersionLib.UnitLibLastUpdateAt = DateTime.Now;
            }
            catch (Exception)
            {
                DDCL.MetaVersionLib.UnitLibLastUpdateAt = DateTime.MinValue;
                return false;
            }
            return true;
           
        }

    }
}
