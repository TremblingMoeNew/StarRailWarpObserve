using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Library.Settings.Models;
using DodocoTales.SR.Library.UnitLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.UnitLibrary
{
    public class DDCLUnitLibrary
    {
        public readonly string libPath = @"Library/UnitLibrary.json";
        List<DDCLUnitItem> model;
        
        Dictionary<string, DDCLUnitItem> dict;

        public DDCLUnitLibrary()
        {
            model = null;
            dict = new Dictionary<string, DDCLUnitItem>();
        }

        public bool IsLoaded()
        {
            return model != null;
        }

        public bool Enabled()
        {
            return DDCL.SettingsLib.MetadataSource != DDCLUnitMetadataSource.None;
        }

        public async Task<bool> LoadLibraryAsync()
        {
            dict.Clear();
            if (!Enabled())
            {
                model = null;
                return true;
            }
            try
            {
                var stream = File.Open(libPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(stream);
                var buffer = await reader.ReadToEndAsync();
                model = JsonConvert.DeserializeObject<List<DDCLUnitItem>>(buffer);
            }
            catch (Exception)
            {
                model = null;
                return false;
            }
            foreach (var item in model)
            {
                dict.Add(item.ItemID, item);
            }
            return true;
        }

        public DDCLUnitItem GetUnitInfo(string item_id)
        {
            return dict.TryGetValue(item_id, out var unitInfo) ? unitInfo : null;
        }
    }
}
