using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Loader;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.ViewModels.Screens
{
    public class DDCVSettingsWindowChangeLogScreenVM : ObservableObject
    {
        private string changelog;
        public string ChangeLog
        {
            get => changelog;
            set => SetProperty(ref changelog, value);
        }

        public async void Refresh()
        {
            if (DDCG.UpdateLoader.ChangeLog == null)
            {
                await DDCG.UpdateLoader.LoadChangeLog();
            }
            if (DDCG.UpdateLoader.ChangeLog == null)
            {
                Notice.Show("更新日志载入失败。", "错误", MessageBoxIcon.Error);
            }
            ChangeLog = DDCG.UpdateLoader.ChangeLog;
        }
    }
}
