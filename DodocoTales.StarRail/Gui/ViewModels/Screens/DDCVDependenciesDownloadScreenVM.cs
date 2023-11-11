using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Common;
using DodocoTales.SR.Loader.Models;
using Panuon.UI.Silver;
using SkiaSharp.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DodocoTales.SR.Gui.ViewModels.Screens
{
    public class DDCVDependenciesDownloadScreenVM : ObservableObject
    {

        private readonly string initialHint = "正在初始化下载……";
        private readonly string failedHint = "下载失败，请稍后再试或尝试手动下载";

        private string downloadHint;
        public string DownloadHint
        {
            get => downloadHint;
            set => SetProperty(ref downloadHint, value);
        }

        private int progressBarLength;
        public int ProgressBarLength
        {
            get => progressBarLength;
            set => SetProperty(ref progressBarLength, value);
        }

        private int progressBarMaxLength;
        public int ProgressBarMaxLength
        {
            get => progressBarMaxLength;
            set => SetProperty(ref progressBarMaxLength, value);
        }


        public DDCVDependenciesDownloadScreenVM()
        {
            ProgressBarLength = 0;
            ProgressBarMaxLength = 500;
            DownloadHint = initialHint;

            DDCS.ClientUpdateDownloadStatusReport += OnDependencyUpdateStatusReported;
            DDCS.DependencyUpdateDownloadFailed += OnDependecyUpdateFailed;
        }

        private void OnDependencyUpdateStatusReported(dynamic var)
        {
            DDCGDownloadTask task = var;

            DownloadHint = $"{task.LocalFileName}  {task.DownloadedSize / 1024.0:F2}KB/{task.FileSize / 1024.0:F2}KB {task.EstSpeed / 1024:F2}KB/s EST:{(task.FileSize - task.DownloadedSize) / task.EstSpeed:F2}s";
            ProgressBarLength = Convert.ToInt32(ProgressBarMaxLength * (1.0 * task.DownloadedSize / task.FileSize));
        }

        private void OnDependecyUpdateFailed()
        {
            DownloadHint = failedHint;
        }



    }
}
