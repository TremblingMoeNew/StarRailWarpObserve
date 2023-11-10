using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Loader.Models
{
    public enum DDCGDownloadTaskType
    {
        ClientUpdate,
        DependencyUpdate
    }

    public class DDCGDownloadTask
    {
        public DDCGDownloadTaskType TaskType { get; set; }
        public string RemoteFileName { get; set; }
        public string LocalFileName { get; set; }
        public long? FileSize { get; set; }
        public long? DownloadedSize { get; set; }
        public double EstSpeed { get; set; }
    }
}
