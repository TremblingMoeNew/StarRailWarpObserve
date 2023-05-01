using DodocoTales.SR.Common;
using DodocoTales.SR.Library.Settings.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.Settings
{
    public class DDCLSettingsLibrary
    {
        public DDCLSettingsInfo model = null;
        public readonly string libPath = @"Library/Settings.json";

        public long LastUserUID { get { return model?.LastUserUID ?? -1; } set { model.LastUserUID = value; SaveSettings(); } }

        public DDCLSettingsLibrary()
        {
            DDCS.CurUserSwapCompleted += OnUIDSwapCompleted;
        }

        public bool IsLoaded()
        {
            return model != null;
        }

        public async Task<bool> LoadSettingsAsync()
        {
            try
            {
                var stream = File.Open(libPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(stream);
                var buffer = await reader.ReadToEndAsync();
                model = JsonConvert.DeserializeObject<DDCLSettingsInfo>(buffer);
            }
            catch (Exception)
            {
                model = new DDCLSettingsInfo();
                return false;
            }
            return true;
        }
        public bool SaveSettings()
        {
            if (model == null) return false;
            try
            {
                var stream = File.Open(libPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
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



        private void OnUIDSwapCompleted(long uid)
        {
            LastUserUID = uid;
            SaveSettings();
        }
    }
}
