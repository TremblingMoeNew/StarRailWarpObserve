using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Loader;
using DodocoTales.SR.Loader.Models;
using Newtonsoft.Json;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DodocoTales.SR.Gui.ViewModels.Dialogs
{
    public class DDCVImportDialogVM : ObservableObject
    {
        private string importPath;
        public string ImportPath
        {
            get => importPath;
            set => SetProperty(ref importPath, value);
        }


        private long selectedUID;
        public long SelectedUID
        {
            get => selectedUID;
            set => SetProperty(ref selectedUID, value);
        }

        private bool isNewUID;
        public bool IsNewUID
        {
            get => isNewUID;
            set => SetProperty(ref isNewUID, value);
        }


        private string selectedLanguage;
        public string SelectedLanguage
        {
            get => selectedLanguage;
            set => SetProperty(ref selectedLanguage, value);
        }

        private DDCLGameClientType selectedClientType;
        public DDCLGameClientType SelectedClientType
        {
            get => selectedClientType;
            set => SetProperty(ref selectedClientType, value);
        }

        private string exportTime;
        public string ExportTime
        {
            get => exportTime;
            set => SetProperty(ref exportTime, value);
        }

        private string application;
        public string Application
        {
            get => application;
            set => SetProperty(ref application, value);
        }

        private string applicationVersion;
        public string ApplicationVersion
        {
            get => applicationVersion;
            set => SetProperty(ref applicationVersion, value);
        }

        private int timeZone;
        public int TimeZone
        {
            get => timeZone;
            set => SetProperty(ref timeZone, value);
        }

        private string standardVersion;
        public string StandardVersion
        {
            get => standardVersion;
            set => SetProperty(ref standardVersion, value);
        }





        public string SelectLogFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "UIGF Organization - Star Rail Gacha Log Format|*.json",
             };
            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                return fileDialog.FileName;
            }
            return null;
        }


        public bool LoadMetadata(DDCGUniversalFormatLogInfo Info)
        {
            if (Info.TimeZone == null) return false;
            try
            {
                SelectedUID = Convert.ToInt64(Info.UID);
                SelectedLanguage = Info.Language;

                IsNewUID = !DDCL.UserDataLib.UserExists(SelectedUID);
                if (IsNewUID)
                {
                    SelectedClientType = DDCG.UFImporter.ConvertGameBizStringToGameClientType(Info.GameBiz);
                    TimeZone = Convert.ToInt32(Info.TimeZone);
                }
                else
                {
                    var userlog = DDCL.UserDataLib.GetUserLogByUid(SelectedUID);
                    SelectedClientType = userlog.ClientType;
                    TimeZone = userlog.TimeZone;
                }

                if (Info.ExportTime != null)
                {
                    ExportTime = Info.ExportTime;
                }
                else if (Info.ExportTimestamp != null)
                {
                    if(Int64.TryParse(Info.ExportTimestamp, out long timestamp))
                    {
                        var tm = DDCL.UnixTimestampToTime(Convert.ToInt64(timestamp));
                        ExportTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}",tm);
                    }
                        
                }
                
                Application = GetApplicationName(Info.Application);
                ApplicationVersion = Info.ApplicationVersion;
                StandardVersion = Info.StandardVersion;
            }
            catch
            {
                return false;
            }
            return true;
        }


        public async Task<bool> LoadLog()
        {
            ImportPath = SelectLogFile();
            if (ImportPath == null) return false;

            var UFlog = await DDCG.UFImporter.Load(ImportPath);

            ImportPath = ImportPath.Replace('\\', '/');
            if (!DDCG.UFImporter.IsAcceptableFormat(UFlog))
            {
                Notice.Show("跃迁记录导入失败。\n选择的文件并非受支持的类型。\n请确保您导入的是SRGF格式的跃迁历史记录文件。", "跃迁记录导入", Panuon.UI.Silver.MessageBoxIcon.Error);
                return false;
            }
            if (!DDCG.UFImporter.IsAcceptableLanguage(UFlog))
            {
                Notice.Show("跃迁记录导入失败。\n当前仅支持导入简体中文语言的跃迁记录。", "跃迁记录导入", Panuon.UI.Silver.MessageBoxIcon.Error);
                return false;
            }

            if (DDCG.UFImporter.IsAnonymousFormat(UFlog))
            {
                Notice.Show("跃迁记录导入失败。\n当前不支持匿名格式导入。", "跃迁记录导入", Panuon.UI.Silver.MessageBoxIcon.Error);
                return false;
            }

            if (!LoadMetadata(UFlog.Info))
            {
                Notice.Show("跃迁记录导入失败。\n元数据残缺或格式错误。", "跃迁记录导入", Panuon.UI.Silver.MessageBoxIcon.Error);
                return false;
            }

            Console.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
            return true;
        }

        public string GetApplicationName(string code)
        {
            return KnownApplication.ContainsKey(code) ? KnownApplication[code] : code;
        }


        private Dictionary<string, string> KnownApplication = new Dictionary<string, string>
        {
            { "DodocoTales.StarRail", "星穹铁道跃迁观测工具" },
        };
    }
}
