using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Gui.Models;
using DodocoTales.SR.Gui.Views.Screens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DodocoTales.SR.Gui.ViewModels.Windows
{
    public class DDCVSettingsWindowVM : ObservableObject
    {
        public readonly Dictionary<string, DDCVSwapableScreen> Screens = new Dictionary<string, DDCVSwapableScreen>();
        public Grid Navigater;
        public DDCVSwapableScreen Current;

        private ObservableCollection<DDCVMainPanelItemModel> menuItems;
        public ObservableCollection<DDCVMainPanelItemModel> MenuItems
        {
            get => menuItems;
            set => SetProperty(ref menuItems, value);
        }

        public DDCVSettingsWindowVM()
        {
            Screens = new Dictionary<string, DDCVSwapableScreen>();

            MenuItems = new ObservableCollection<DDCVMainPanelItemModel>()
            {
                new DDCVMainPanelItemModel("用户","Users", 0),
                new DDCVMainPanelItemModel("关于","About",0),
                new DDCVMainPanelItemModel("更新日志","ChangeLog",0),
            };
        }

        public void Initialize(Grid navigator)
        {
            Navigater = navigator;
            RegisterScreen("Users", new DDCVSettingsWindowUsersScreen());
            RegisterScreen("About", new DDCVSettingsWindowAboutScreen());
            RegisterScreen("ChangeLog", new DDCVSettingsWindowChangeLogScreen());
            SelectMenu("About");
        }

        public void RegisterScreen(string tag, DDCVSwapableScreen screen)
        {
            Screens.Add(tag, screen);
            screen.Visibility = Visibility.Collapsed;
            Navigater.Children.Add(screen);
        }

        public void SwapScreen(string tag)
        {
            if (!Screens.TryGetValue(tag, out DDCVSwapableScreen ns))
            {
                return;
            }
            if(Current != null)
            {
                Current.Visibility = Visibility.Collapsed;
            }
            Current = ns;
            Current.Visibility=Visibility.Visible;
            Current.Refresh();
        }

        public void SelectMenu(string tag)
        {
            MenuItems.ToList().ForEach(x=>x.IsSelected = (x.Tag == tag));
        }
    }
}
