using DodocoTales.SR.Library.GameClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.GameClient
{
    public class DDCLGameClientLibrary
    {
        private List<DDCLGameClientItem> clients;
        private DDCLGameClientItem SelectedClient;

        public readonly string libPath = @"library/GameClientLibrary.json";

        public DDCLGameClientLibrary()
        {
            clients = new List<DDCLGameClientItem>();
        }


        public DDCLGameClientItem GetSelectedClient()
        {
            if (SelectedClient == null)
            {
                if (clients.Count == 1)
                {
                    SelectedClient = clients.First();
                }
                if (clients.Count(x => x.IsDefault) == 1)
                {
                    SelectedClient = clients.Find(x => x.IsDefault);
                }
            }
            return SelectedClient;
        }
        public void SetSelectedClient(DDCLGameClientItem item)
        {
            if (clients.Contains(item)) SelectedClient = item; else SelectedClient = null;
        }

        public List<DDCLGameClientItem> GetClients()
        {
            return clients;
        }

        public void AddClients(List<DDCLGameClientItem> clients)
        {
            this.clients.AddRange(clients);
            SaveLibraryAsync();
            //DDCS.Emit_GameClientLibReloadCompleted();
        }

        public void RemoveClients(List<DDCLGameClientItem> clients)
        {
            this.clients.RemoveAll(x => clients.Contains(x));
            SaveLibraryAsync();
            //DDCS.Emit_GameClientLibReloadCompleted();
        }


        public async Task<bool> LoadLibraryAsync()
        {
            clients.Clear();
            try
            {
                var stream = File.Open(libPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(stream);
                var buffer = await reader.ReadToEndAsync();
                reader.Close();
                stream.Close();
                clients = JsonConvert.DeserializeObject<List<DDCLGameClientItem>>(buffer);
            }
            catch (Exception)
            {
                clients = new List<DDCLGameClientItem>();
                //DDCLog.Error(DCLN.Lib, "GameClientLib damaged or not exist. Reset to default.");
                SaveLibraryAsync();
            }
            //DDCLog.Info(DCLN.Lib, "GameClientLib successfully loaded.");
            //DDCS.Emit_GameClientLibReloadCompleted();
            return true;
        }

        public async Task SaveLibraryAsync()
        {
            try
            {
                using (var stream = File.Open(libPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    StreamWriter writer = new StreamWriter(stream);
                    var serialized = JsonConvert.SerializeObject(clients, Formatting.Indented);
                    await writer.WriteAsync(serialized);
                    await writer.FlushAsync();
                }
                //DDCLog.Info(DCLN.Lib, "GameClientLib successfully saved.");
            }
            catch (Exception e)
            {
                //DDCLog.Error(DCLN.Lib, "Failed to save GameClientLib. ", e);
            }

        }
    }
}
