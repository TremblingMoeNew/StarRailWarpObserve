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

        private Dictionary<long, bool> uidOptionsMulti;
        public Dictionary<long, bool> UIDOptionsMulti
        {
            get => uidOptionsMulti;
            set => SetProperty(ref uidOptionsMulti, value);
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

        private List<long> selectedUIDMulti;
        public List<long> SelectedUIDMulti
        {
            get => selectedUIDMulti;
            set => SetProperty(ref selectedUIDMulti, value);
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
                {"JSON: 新版统一可交换抽卡记录格式(New UIGF)", DDCVSupportedGachaLogFormat.NewUniversalGachaLogFormat },
                {"JSON: 星穹铁道抽卡记录格式(SRGF)", DDCVSupportedGachaLogFormat.StarRailGachaLogFormat },
                {"JSON: 通用抽卡记录格式-最大化兼容(New UIGF+SRGF)", DDCVSupportedGachaLogFormat.DualFormat_NewUIGF_SRGF },
            };
            FormatType = DDCVSupportedGachaLogFormat.StarRailGachaLogFormat;
            UIDOptions = DDCL.UserDataLib.U.Keys.ToList();
            UIDOptionsMulti = new Dictionary<long, bool>();
            UIDOptions.ForEach(key => { UIDOptionsMulti.Add(key, false); });
            
            SelectedUID = DDCL.CurrentUser.OriginalLogs?.UID ?? - 1;
            SelectedUIDMulti = new List<long>();
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
                    ExportPath = (fileInfo.DirectoryName + "/" + DDCG.UFExporter.GenerateLegacyExportFileName(SelectedUID)).Replace('\\','/');
                    break;
                case DDCVSupportedGachaLogFormat.NewUniversalGachaLogFormat:
                    ExportPath = (fileInfo.DirectoryName + "/" + DDCG.UFExporter.GenerateMultiExportFileName()).Replace('\\', '/');
                    break;
                case DDCVSupportedGachaLogFormat.DualFormat_NewUIGF_SRGF:
                    ExportPath = (fileInfo.DirectoryName + "/" + DDCG.UFExporter.GenerateDualExportFileName(SelectedUID)).Replace('\\', '/');
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
                case DDCVSupportedGachaLogFormat.NewUniversalGachaLogFormat:
                    saveFileDialog.Filter = "UIGF Organization - New Uniformed Interchangeable Gacha Log Format|*.json";
                    break;
                case DDCVSupportedGachaLogFormat.DualFormat_NewUIGF_SRGF:
                    saveFileDialog.Filter = "Starwo Custom - New UIGF-Legacy SRGF Dual Format|*.json";
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
            bool res = true;
            switch (FormatType)
            {
                case DDCVSupportedGachaLogFormat.StarRailGachaLogFormat:
                {
                    if (ExportPath == null || SelectedUID == -1) return false;
                    await DDCG.UFExporter.Export(ExportPath, new List<long> { SelectedUID }, false, true, false);
                    break;
                }
                case DDCVSupportedGachaLogFormat.DualFormat_NewUIGF_SRGF:
                {
                    if (ExportPath == null || SelectedUID == -1) return false;
                    await DDCG.UFExporter.Export(ExportPath, new List<long> { SelectedUID }, true, true, false);
                    break;
                }
                case DDCVSupportedGachaLogFormat.NewUniversalGachaLogFormat:
                {
                    if (ExportPath == null || SelectedUIDMulti == null || SelectedUIDMulti.Count == 0) return false;
                    await DDCG.UFExporter.Export(ExportPath, SelectedUIDMulti, true, false, false);
                    break;
                }
                default:
                {
                    return false;
                }
            }

            if(res)
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
