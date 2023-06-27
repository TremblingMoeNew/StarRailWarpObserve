using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Gui.Models;
using DodocoTales.SR.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.ViewModels.Screens
{
    public class DDCVSettingsWindowUsersScreenVM : ObservableObject
    {
        private ObservableCollection<DDCVUserlogItem> users;
        public ObservableCollection<DDCVUserlogItem> Users
        {
            get => users;
            set => SetProperty(ref users, value);
        }


        public void Refresh()
        {
            List<DDCVUserlogItem> list = new List<DDCVUserlogItem>();
            foreach (var log in DDCL.UserDataLib.U.Values)
            {
                list.Add(new DDCVUserlogItem
                {
                    UID = log.UID,
                    ClientType = log.ClientType,
                    TimeZone = log.TimeZone,
                    IsSelected = DDCL.CurrentUser.IsCurrentUser(log.UID),
                });
            }
            Users = new ObservableCollection<DDCVUserlogItem>(list);

        }

        public void SwapUser(DDCVUserlogItem user)
        {
            DDCL.CurrentUser.SwapUser(user.UID);
            Refresh();
        }

        public async void RemoveUser(DDCVUserlogItem user)
        {
            await DDCL.UserDataLib.RemoveUserByUid(user.UID);
            Refresh();
        }

    }
}
