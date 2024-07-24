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

        private int currentSectionIdx;
        public int CurrentSectionIdx
        {
            get => currentSectionIdx;
            set => SetProperty(ref currentSectionIdx, value);
        }
        private int sectionCnt;
        public int SectionCnt
        {
            get => sectionCnt;
            set => SetProperty(ref sectionCnt, value);
        }


        public DDCVImportDialogVM()
        {
            FormatOptions = new Dictionary<string, DDCVSupportedGachaLogFormat>
            {
                {"JSON: 新版统一可交换抽卡记录格式(New UIGF)", DDCVSupportedGachaLogFormat.NewUniversalGachaLogFormat },
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
                Filter = "UIGF Organization - New Uniformed Interchangeable Gacha Log Format|*.json|UIGF Organization - Star Rail Gacha Log Format|*.json",
             };
            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                return fileDialog.FileName;
            }
            return null;
        }


        public bool LoadMetadata(DDCGUniversalFormatLogInfo Info)
        {
            try
            {
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

                if (Info.NewStandardVersion != null)
                {
                    StandardVersion = Info.NewStandardVersion;
                    FormatType = DDCVSupportedGachaLogFormat.NewUniversalGachaLogFormat;
                }
                else if (Info.LegacyStandardVersion != null)
                {
                    StandardVersion = Info.LegacyStandardVersion;
                    FormatType = DDCVSupportedGachaLogFormat.StarRailGachaLogFormat;
                }          
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }


        public bool LoadSectionMetadata(DDCGUniversalFormatLogSRSection section)
        {
            try
            {
                SelectedUID = Convert.ToInt64(section.UID);
                SelectedLanguage = section.Language;

                IsNewUID = !DDCL.UserDataLib.UserExists(SelectedUID);
                if (IsNewUID)
                {
                    SelectedClientType = DDCG.UFImporter.ConvertGameBizStringToGameClientType(section.GameBiz);
                    TimeZone = Convert.ToInt32(section.TimeZone);
                }
                else
                {
                    var userlog = DDCL.UserDataLib.GetUserLogByUid(SelectedUID);
                    SelectedClientType = userlog.ClientType;
                    TimeZone = userlog.TimeZone;
                }
                
            }
            catch { return false; }
            return true;
        }


        public bool LoadNextSection()
        {
            while (CurrentSectionIdx < SectionCnt)
            {
                var section = UFlog.StarRailSections[CurrentSectionIdx];
                CurrentSectionIdx++;
                if (!DDCG.UFImporter.IsAcceptableSection(section))
                {
                    Notice.Show($"跃迁记录项导入失败。\n第{CurrentSectionIdx}项的元数据残缺或格式错误，无法导入。", "跃迁记录导入", Panuon.UI.Silver.MessageBoxIcon.Error);
                    continue;
                }
                if (!DDCG.UFImporter.IsAcceptableLanguage(section))
                {
                    Notice.Show($"跃迁记录项导入失败。\n本工具仅支持zh-cn语言记录项，第{CurrentSectionIdx}项无法导入", "跃迁记录导入", Panuon.UI.Silver.MessageBoxIcon.Error);
                    continue;
                }
                if (!LoadSectionMetadata(section))
                {
                    Notice.Show($"跃迁记录项导入失败。\n第{CurrentSectionIdx}项的元数据残缺或格式错误，无法导入。", "跃迁记录导入", Panuon.UI.Silver.MessageBoxIcon.Error);
                    continue;
                }
                return true;
            }
            return false;
        }

        public async Task<bool> LoadLog()
        {
            ImportPath = SelectLogFile();
            if (ImportPath == null) return false;

            UFlog = await DDCG.UFImporter.Load(ImportPath);

            ImportPath = ImportPath.Replace('\\', '/');
            if (!DDCG.UFImporter.IsAcceptableFormat(UFlog))
            {
                Notice.Show("跃迁记录导入失败。\n选择的文件并非受支持的类型。\n请确保您导入的是新版UIGF(且确认为星穹铁道的记录)或SRGF格式的跃迁历史记录文件。", "跃迁记录导入", Panuon.UI.Silver.MessageBoxIcon.Error);
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
            CurrentSectionIdx = 0;
            SectionCnt = UFlog.StarRailSections?.Count ?? 0;
            if (!LoadNextSection())
            {
                return false;
            }

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

        long firstAvailableUid = -1;
        public async Task<bool> Import()
        {
            var list = DDCG.UFImporter.ConvertList(UFlog.StarRailSections[CurrentSectionIdx - 1].List);
            var available_cnt = list.Count;
            var failed_cnt = UFlog.StarRailSections[CurrentSectionIdx - 1].List.Count - available_cnt;
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
            if (firstAvailableUid < 0)
            {
                firstAvailableUid = SelectedUID;
            }
            if (!LoadNextSection())
            {
                if (firstAvailableUid >= 0)
                {
                    DDCL.CurrentUser.SwapUser(firstAvailableUid);
                    DDCS.Emit_CurUserUpdateCompleted();
                }
                return true;
            }
            return false;

        }


    }
}
