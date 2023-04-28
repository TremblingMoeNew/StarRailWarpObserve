using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Library;
using DodocoTales.SR.Library.GameClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.ViewModels.Windows
{
    public class DDCVGameClientManagerWindowVM : ObservableObject
    {
        private ObservableCollection<DDCLGameClientItem> clients;
        public ObservableCollection<DDCLGameClientItem> Clients
        {
            get => clients;
            set => SetProperty(ref clients, value);
        }
        public void ReloadData()
        {
            Clients = new ObservableCollection<DDCLGameClientItem>(DDCL.GameClientLib.GetClients());
        }


    }
}
