using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Common.Enums;
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
    public class DDCVVersionViewScreenVM : ObservableObject
    {
        private ObservableCollection<DDCVVersionItemModel> versions;
        public ObservableCollection<DDCVVersionItemModel> Versions
        {
            get => versions;
            set => SetProperty(ref versions, value);
        }




        public void ReloadData()
        {
            var ls = new List<DDCVVersionItemModel>();
            var typeEmpty = new Dictionary<DDCCPoolType, bool>();
            foreach (DDCCPoolType type in Enum.GetValues(typeof(DDCCPoolType)))
            {
                typeEmpty[type] = true;
            }
            var tz = DDCL.CurrentUser.GetActivatingTimeZone();
            foreach (var version in DDCL.BannerLib.Versions)
            {
                DDCVVersionItemModel vermodel = new DDCVVersionItemModel
                {
                    VersionID = version.ID,
                    VersionName = version.Name,
                    Version = version.Version,
                    BeginTime = DDCL.GetSyncTimeOffset(version.BeginTime).ToLocalTime(),
                    EndTime = DDCL.GetSyncTimeOffset(version.EndTime).ToLocalTime()
                };
                var r5s = new List<DDCVUnitIndicatorModel>();
                var banls = new List<DDCVBannerItemModel>();
                var banners = DDCL.CurrentUser.GetBannersByVersion(version.ID);
                foreach (var banner in banners)
                {
                    var baninfo = DDCL.BannerLib.GetBanner(banner.BannerInternalID);
                    DDCVBannerItemModel banmodel = new DDCVBannerItemModel
                    {
                        VersionID = version.ID,
                        Version = version.Version,
                        BannerName = baninfo.Name,
                        BannerInternalID = banner.BannerInternalID,
                        BannerHint = baninfo.Hint,
                        PoolType = banner.CategorizedGachaType,
                        BeginTime = DDCL.GetBannerTimeOffset(baninfo.BeginTime, baninfo.BeginTimeSync, tz).ToLocalTime(),
                        EndTime = DDCL.GetBannerTimeOffset(baninfo.EndTime, baninfo.EndTimeSync, tz).ToLocalTime(),

                    };

                    banmodel.Total = banner.Logs.Count;

                    if (banmodel.Total > 0)
                    {
                        typeEmpty[banner.CategorizedGachaType] = false;
                    }

                    var r5 = banner.Logs.FindAll(x => x.Rank == 5);
                    banmodel.Rank5 = r5.Count;
                    var r5ups = new List<DDCVUnitIndicatorModel>();
                    foreach (var up in baninfo.Rank5Up)
                    {
                        r5ups.Add(new DDCVUnitIndicatorModel
                        {
                            Name = up,
                            Count = r5.FindAll(x => x.Name == up).Count
                        });
                    }
                    banmodel.Rank5Ups = new ObservableCollection<DDCVUnitIndicatorModel>(r5ups);
                    foreach (var item in r5)
                    {
                        r5s.Add(new DDCVUnitIndicatorModel
                        {
                            Name = item.Name,
                            Time = DDCL.GetTimeOffset(item.Time, tz),
                            Version = version.Version,
                            Banner = baninfo.Name,
                            ID = item.ID,
                            Count = DDCL.CurrentUser.Logs.Values.Where(x => x.RoundID == item.RoundID).Count()
                        });
                    }


                    var r4 = banner.Logs.FindAll(x => x.Rank == 4);
                    banmodel.Rank4 = r4.Count;
                    var r4ups = new List<DDCVUnitIndicatorModel>();
                    foreach (var up in baninfo.Rank4Up)
                    {
                        r4ups.Add(new DDCVUnitIndicatorModel
                        {
                            Name = up,
                            Count = r4.FindAll(x => x.Name == up).Count
                        });
                    }
                    r4ups.Sort((x, y) => x.Name.Length.CompareTo(y.Name.Length));
                    banmodel.Rank4Ups = new ObservableCollection<DDCVUnitIndicatorModel>(r4ups);


                    if (!typeEmpty[banner.CategorizedGachaType])
                    {
                        banls.Add(banmodel);
                    }
                    typeEmpty[DDCCPoolType.Beginner] = typeEmpty[DDCCPoolType.Null] = true;
                }


                if (banls.Count > 0)
                {
                    banls.Reverse();
                    vermodel.Banners = new ObservableCollection<DDCVBannerItemModel>(banls);
                    r5s.Sort((x, y) => y.ID.CompareTo(x.ID));
                    vermodel.Rank5s = new ObservableCollection<DDCVUnitIndicatorModel>(r5s);
                    ls.Add(vermodel);
                }


            }
            ls.Reverse();
            Versions = new ObservableCollection<DDCVVersionItemModel>(ls);
        }


    }
}
