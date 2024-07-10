using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Common;
using DodocoTales.SR.Gui.Enums;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using DodocoTales.SR.Library.TrustedExporters.Models;
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
        private DDCGUniversalFormatLog UFlog;

        private Dictionary<string, DDCVSupportedGachaLogFormat> formatOptions;
        public Dictionary<string, DDCVSupportedGachaLogFormat> FormatOptions
        {
            get => formatOptions;
            set => SetProperty(ref formatOptions, value);
        }

        private DDCVSupportedGachaLogFormat formatType;
        public DDCVSupportedGachaLogFormat FormatType
        {
            get => formatType;
            set => SetProperty(ref formatType, value);
        }
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

        private Dictionary<string, DDCLGameClientType> clientTypeOptions;
        public Dictionary<string, DDCLGameClientType> ClientTypeOptions
        {
            get => clientTypeOptions;
            set => SetProperty(ref clientTypeOptions, value);
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

        private DDCLTrustedExporter application;
        public DDCLTrustedExporter Application
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


        public DDCVImportDialogVM()
        {
            FormatOptions = new Dictionary<string, DDCVSupportedGachaLogFormat>
            {
                {"JSON: 星穹铁道抽卡记录格式(SRGF)", DDCVSupportedGachaLogFormat.StarRailGachaLogFormat },
                //{"JSON: 星穹铁道抽卡记录格式(SRGF)兼容 - 匿名", DDCVSupportedGachaLogFormat.StarRailGachaLogFormatAnonymous },
            };
            FormatType = DDCVSupportedGachaLogFormat.StarRailGachaLogFormat;
            ClientTypeOptions = new Dictionary<string, DDCLGameClientType>
            {
                { "国服客户端", DDCLGameClientType.CN },
                { "国际服客户端", DDCLGameClientType.Global }
            };
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

                Application = DDCL.ExportersLib.GetExporter(Info.Application);
                ApplicationVersion = Info.ApplicationVersion;
                StandardVersion = Info.LegacyStandardVersion;
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

            UFlog = await DDCG.UFImporter.Load(ImportPath);

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
            var exporter = DDCL.ExportersLib.GetExporter(code);

            return $"{exporter.ApplicationNameChinese} ({exporter.Author})";
        }

        private int comfirmCounter = 0;
        public bool IsImportReady()
        {
            if (SelectedClientType == DDCLGameClientType.Unknown)
            {
                Notice.Show($"请先选择该UID所属的客户端类型（国服或国际服）。", "跃迁记录导入", Panuon.UI.Silver.MessageBoxIcon.Error);
                return false;
            }
            else if (Application.ExporterType == DDCLExporterType.Blacklist && comfirmCounter == 0)
            {
                comfirmCounter++;
                Notice.Show($"若确认要从不可靠来源继续导入数据，请再次点击“导入”键。\n本次导入的数据将无法被导出。Starwo不对任何可能产生的错误负责。", "从不可靠来源导入数据", Panuon.UI.Silver.MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        public async Task Import()
        {
            var list = DDCG.UFImporter.ConvertList(UFlog.List);
            var available_cnt = list.Count;
            var failed_cnt = UFlog.List.Count - available_cnt;
            if (Application.ExporterType == DDCLExporterType.Blacklist)
            {
                if(DDCL.UserDataLib.UserExists(SelectedUID))
                {
                    await DDCL.UserDataLib.BackupUserAsync(DDCL.UserDataLib.GetUserLogByUid(SelectedUID));
                }
                list.ForEach(x => x.Untrusted = true);
                Notice.Show($"用户{SelectedUID}原有跃迁记录文件已经备份。\n本次导入的数据将无法被导出。", "从不可靠来源导入数据", Panuon.UI.Silver.MessageBoxIcon.Info);
            }
            var added_cnt = DDCG.UFImporter.Import(SelectedUID, list, SelectedClientType, TimeZone, Application.Application);
            Notice.Show($"跃迁记录导入完毕。\n{available_cnt}个记录项读取成功，{failed_cnt}个记录项读取失败。\n用户{SelectedUID}新增{added_cnt}个跃迁记录项。", "跃迁记录导入", Panuon.UI.Silver.MessageBoxIcon.Success);
            DDCL.CurrentUser.SwapUser(SelectedUID);
            DDCS.Emit_CurUserUpdateCompleted();
        }


    }
}
