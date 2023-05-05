using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Library.GameClient.Models;
using DodocoTales.SR.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.ViewModels.Dialogs
{
    public class DDCVGameClientManagerWindowEditDialogVM : ObservableObject
    {
        private Dictionary<string, DDCLGameClientType> clientOptions;
        public Dictionary<string, DDCLGameClientType> ClientOptions
        {
            get => clientOptions;
            set => SetProperty(ref clientOptions, value);
        }
        private Dictionary<string, int> timeZoneOptions;
        public Dictionary<string, int> TimeZoneOptions
        {
            get => timeZoneOptions;
            set => SetProperty(ref timeZoneOptions, value);
        }

        private DDCLGameClientType clientType;
        public DDCLGameClientType ClientType
        {
            get => clientType;
            set
            {
                SetProperty(ref clientType, value);
                switch(ClientType)
                {
                    case DDCLGameClientType.CN:
                        if(string.IsNullOrEmpty(Name)|| Name== DDCG.GameClientLoader.ClientName_OS)
                            Name = DDCG.GameClientLoader.ClientName_CN;
                        break;
                    case DDCLGameClientType.Global:
                        if (string.IsNullOrEmpty(Name) || Name == DDCG.GameClientLoader.ClientName_CN)
                            Name = DDCG.GameClientLoader.ClientName_OS;
                        break;
                }
            }
        }
        private int timeZone;
        public int TimeZone
        {
            get => timeZone;
            set => SetProperty(ref timeZone, value);
        }

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string path;
        public string Path
        {
            get => path;
            set => SetProperty(ref path, value);
        }

        public DDCLGameClientItem GameClientItem { get; set; }

        public DDCVGameClientManagerWindowEditDialogVM()
        {
            ClientOptions = new Dictionary<string, DDCLGameClientType>
            {
                { "国服客户端", DDCLGameClientType.CN },
                { "国际服客户端", DDCLGameClientType.Global }
            };

        }

        public void NewGameClientItem()
        {
            GameClientItem = null;
        }

        public void LoadGameClientItem(DDCLGameClientItem item)
        {
            GameClientItem = item;
            Name = item.Name;
            ClientType = item.ClientType;
            Path = item.Path;
        }

        readonly string filefilter = "Honkai Star Rail Executable|StarRail.exe";
        public void EditGamePath()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = filefilter,

            };
            if (Path != null) dialog.InitialDirectory = Directory.GetParent(Path).Parent.FullName;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var item = DDCG.GameClientLoader.LoadGameClientItemFromExecutablePath(dialog.FileName);
                if (item != null)
                {
                    if (Name == null) Name = item.Name;
                    if (ClientType == DDCLGameClientType.Unknown) ClientType = item.ClientType;
                    Path = item.Path;
                }

            }
        }
        public bool CheckValid()
        {
            return !string.IsNullOrEmpty(Name)
                && ClientType != DDCLGameClientType.Unknown
                && !string.IsNullOrEmpty(Path);
        }


        public bool SaveAsCopy()
        {
            if (!CheckValid()) return false;
            var item = new DDCLGameClientItem
            {
                Name = Name,
                ClientType = ClientType,
                Path = Path,
            };
            DDCL.GameClientLib.AddClients(new List<DDCLGameClientItem> { item });
            return true;
        }

        public async Task<bool> Save()
        {
            if (GameClientItem == null)
            {
                return SaveAsCopy();
            }
            if (!CheckValid()) return false;
            GameClientItem.Name = Name;
            GameClientItem.ClientType = ClientType;
            GameClientItem.Path = Path;
            await DDCL.GameClientLib.SaveLibraryAsync();
            return true;
        }
        public void Delete()
        {
            if (GameClientItem == null) return;
            DDCL.GameClientLib.RemoveClients(new List<DDCLGameClientItem> { GameClientItem });
        }
    }
}
