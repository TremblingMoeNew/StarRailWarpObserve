using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Library.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodocoTales.SR.Library.BannerLibrary.Models
{
    public class DDCLBannerInfo
    {
        public ulong ID { get; set; }
        public DDCCPoolType Type { get; set; }
        public string Name { get; set; }
        public string Hint { get; set; }
        public List<string> Rank5Up { get; set; }
        public List<string> Rank4Up { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool BeginTimeSync { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool EndTimeSync { get; set; }

        [JsonIgnore]
        public ulong VersionId { get; set; }

        [JsonIgnore]
        public ulong InternalId
        {
            get { return VersionId * 1000 + ID; }
        }

        public DDCLBannerInfo Copy()
        {
            return new DDCLBannerInfo
            {
                ID = this.ID,
                Type = this.Type,
                Name = this.Name,
                Hint = this.Hint,
                Rank5Up = this.Rank5Up.FindAll(x => true),
                Rank4Up = this.Rank4Up.FindAll(x => true),
                BeginTime = this.BeginTime,
                EndTime = this.EndTime,
                BeginTimeSync = this.BeginTimeSync,
                EndTimeSync = this.EndTimeSync,
                VersionId = this.VersionId,
            };
        }


        [JsonIgnore]
        public DDCLActivateStatus ActivateStatus
        {
            get
            {
                var now = DDCL.GetNowDateTimeOffset();
                return BannerStatusAtTime(now);
            }
        }

        public DDCLActivateStatus BannerStatusAtTime(DateTimeOffset time)
        {
            var tz = DDCL.DefaultTimeZone;//Temp //DDCL.CurrentUser.GetActivatingTimeZone();
            var begin = DDCL.GetBannerTimeOffset(BeginTime, BeginTimeSync, tz);
            var end = DDCL.GetBannerTimeOffset(EndTime, EndTimeSync, tz);
            var res = DDCL.CheckTimeIsBetween(begin, end, time);
            if (res == 0) return DDCLActivateStatus.Activating;
            else if (res < 0) return DDCLActivateStatus.Post;
            else return DDCLActivateStatus.Incoming;
        }
    }
}
