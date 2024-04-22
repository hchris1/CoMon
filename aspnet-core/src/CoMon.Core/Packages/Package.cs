using Abp.Domain.Entities;
using CoMon.Assets;
using CoMon.Packages.Settings;
using CoMon.Statuses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoMon.Packages
{
    [Table("CoMonPackages")]
    public class Package : Entity<long>
    {
        public const int MaxNameLength = 256;

        [Required]
        [StringLength(MaxNameLength, MinimumLength = 1)]
        public string Name { get; set; }
        public PackageType Type { get; set; }
        public List<Status> Statuses { get; set; } = [];
        public PingPackageSettings PingPackageSettings { get; set; }
        public HttpPackageSettings HttpPackageSettings { get; set; }
        public RtspPackageSettings RtspPackageSettings { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(Asset))]
        public long AssetId { get; set; }
        public Asset Asset { get; set; }

        [NotMapped]
        public Status LastStatus
        {
            get
            {
                return Statuses.OrderByDescending(s => s.Time).FirstOrDefault();
            }
        }

        [NotMapped]
        public Criticality? LastCriticality
        {
            get
            {
                return LastStatus?.Criticality ?? null;
            }
        }

        public IPackageSettings GetSettings<TSettings>() where TSettings : class
        {
            if (Type == PackageType.Ping)
                return PingPackageSettings;
            if (Type == PackageType.Http)
                return HttpPackageSettings;
            if (Type == PackageType.Rtsp)
                return RtspPackageSettings;
            return null;
        }
    }

    public enum PackageType
    {
        Ping = 0,
        Http = 1,
        Rtsp = 2,
        External = 10
    }
}
