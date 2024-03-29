﻿using CommunityToolkit.Mvvm.ComponentModel;
using DodocoTales.SR.Gui.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Gui.Models
{
    public class DDCVUnitIndicatorModel : ObservableObject
    {
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private int count;
        public int Count
        {
            get => count;
            set => SetProperty(ref count, value);
        }

        private DateTimeOffset time;
        public DateTimeOffset Time
        {
            get => time;
            set => SetProperty(ref time, value);
        }

        private string version;
        public string Version
        {
            get => version;
            set => SetProperty(ref version, value);
        }
        private string banner;
        public string Banner
        {
            get => banner;
            set => SetProperty(ref banner, value);
        }
        private int index;
        public int Index
        {
            get => index;
            set => SetProperty(ref index, value);
        }
        private DDCVUnitIndicatorType indicatorType;
        public DDCVUnitIndicatorType IndicatorType
        {
            get => indicatorType;
            set => SetProperty(ref indicatorType, value);
        }
        private bool inherited;
        public bool Inherited
        {
            get => inherited;
            set => SetProperty(ref inherited, value);
        }
        private ulong id;
        public ulong ID
        {
            get => id;
            set => SetProperty(ref id, value);
        }
        private bool imported;
        public bool Imported
        {
            get => imported;
            set => SetProperty(ref imported, value);
        }
        private bool untrusted;
        public bool Untrusted
        {
            get => untrusted;
            set => SetProperty(ref untrusted, value);
        }
        private string application;
        public string ImportApplication
        {
            get => application;
            set => SetProperty(ref application, value);
        }

        private int brMax;
        public int BrMax
        {
            get => brMax;
            set => SetProperty(ref brMax, value);
        }
        private int rndMax;
        public int RndMax
        {
            get => rndMax;
            set => SetProperty(ref rndMax, value);
        }
        private bool brlucky;
        public bool BrLucky
        {
            get => brlucky;
            set => SetProperty(ref brlucky, value);
        }

        private bool brunlucky;
        public bool BrUnlucky
        {
            get => brunlucky;
            set => SetProperty(ref brunlucky, value);
        }

        private bool rndlucky;
        public bool RndLucky
        {
            get => rndlucky;
            set => SetProperty(ref rndlucky, value);
        }

        private bool rndunlucky;
        public bool RndUnlucky
        {
            get => rndunlucky;
            set => SetProperty(ref rndunlucky, value);
        }

        private int rndcount;
        public int RndCount
        {
            get => rndcount;
            set => SetProperty(ref rndcount, value);
        }

    }
}
