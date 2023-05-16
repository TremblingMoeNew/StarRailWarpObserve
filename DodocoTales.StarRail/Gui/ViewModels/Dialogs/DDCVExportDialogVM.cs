using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Gui.Enums;
using DodocoTales.SR.Library;
using DodocoTales.SR.Loader;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DodocoTales.SR.Gui.ViewModels.Dialogs
{
    public class DDCVExportDialogVM : ObservableObject
    {
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
            set
            {
                SetProperty(ref formatType, value);
                GenerateExportPath();
            }
        }

        private List<long> uidOptions;
        public List<long> UIDOptions
        {
            get => uidOptions;
            set => SetProperty(ref uidOptions, value);
        }

        private long selectedUID;
        public long SelectedUID
        {
            get => selectedUID;
            set
            {
                SetProperty(ref selectedUID, value);
                GenerateExportPath();
            }
        }

        private string exportPath;
        public string ExportPath
        {
            get => exportPath;
            set
            {
                SetProperty(ref exportPath, value);
            }
        }

        public DDCVExportDialogVM()
        {
            FormatOptions = new Dictionary<string, DDCVSupportedGachaLogFormat>
            {
                //{"XLSX: 星铁跃迁观测工具表格导出格式", DDCVSupportedGachaLogFormat.StarwoWorkbookExportFormat },
                {"JSON: 星穹铁道抽卡记录格式(SRGF)", DDCVSupportedGachaLogFormat.StarRailGachaLogFormat },
                //{"JSON: 星穹铁道抽卡记录格式(SRGF)兼容 - 匿名", DDCVSupportedGachaLogFormat.StarRailGachaLogFormatAnonymous },
            };
            FormatType = DDCVSupportedGachaLogFormat.StarRailGachaLogFormat;

            UIDOptions = DDCL.UserDataLib.U.Keys.ToList();
            SelectedUID = DDCL.CurrentUser.OriginalLogs?.UID ?? - 1;
        }


        public void GenerateExportPath()
        {
            if (SelectedUID == -1)
            {
                ExportPath = null;
                return;
            }
            FileInfo fileInfo = new FileInfo(ExportPath ?? "Export/placeholder");
            switch (FormatType)
            {
                case DDCVSupportedGachaLogFormat.StarRailGachaLogFormat:
                    ExportPath = (fileInfo.DirectoryName + "/" + DDCG.UFExporter.GenerateExportFileName(SelectedUID, false)).Replace('\\','/');
                    break;
                default:
                    ExportPath = null;
                    break;

            }
        }

        public void SelectExportPath()
        {
            if (SelectedUID == -1 || ExportPath == null) return;
            FileInfo fileInfo = new FileInfo(ExportPath);
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = fileInfo.DirectoryName,
                FileName = fileInfo.Name,
            };
            switch (FormatType)
            {
                case DDCVSupportedGachaLogFormat.StarRailGachaLogFormat:
                    saveFileDialog.Filter = "UIGF Organization - Star Rail Gacha Log Format|*.json";
                    break;
                default:
                    return;
            }
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExportPath = saveFileDialog.FileName;
            }
        }

        public async Task<bool> Export()
        {
            if (ExportPath == null || SelectedUID == -1) return false;
            if(await DDCG.UFExporter.Export(ExportPath, SelectedUID, false))
            {
                Notice.Show("跃迁记录导出完毕", "跃迁记录导出", Panuon.UI.Silver.MessageBoxIcon.Success);
            }
            else
            {
                Notice.Show("跃迁记录导出失败", "跃迁记录导出", Panuon.UI.Silver.MessageBoxIcon.Error);
            }
            return true;

        }
    }
}
