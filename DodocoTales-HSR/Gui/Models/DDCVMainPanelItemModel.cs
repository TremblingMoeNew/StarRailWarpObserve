using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DodocoTales.SR.Gui.Models
{
    public class DDCVMainPanelItemModel : ObservableObject
    {
        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string tag;
        public string Tag
        {
            get => tag;
            set => SetProperty(ref tag, value);
        }

        // TODO: Screen's pointer

        private ObservableCollection<DDCVMainPanelItemModel> menuItems;
        public ObservableCollection<DDCVMainPanelItemModel> MenuItems
        {
            get => menuItems;
            set => SetProperty(ref menuItems, value);
        }


        private Thickness padding;
        public Thickness Padding
        {
            get => padding;
            set => SetProperty(ref padding, value);
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }


        public DDCVMainPanelItemModel(string title, string tag, int level)
        {
            Title = title;
            Tag = tag;
            Padding = new Thickness(10 * level, 0, 0, 0);
            MenuItems = new ObservableCollection<DDCVMainPanelItemModel>();
            IsSelected = false;
        }
    }
}
