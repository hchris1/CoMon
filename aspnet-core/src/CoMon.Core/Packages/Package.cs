﻿using Abp.Domain.Entities;
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

        [StringLength(MaxNameLength)]
        public string Name { get; set; }
        public PackageType Type { get; set; }
        public List<Status> Statuses { get; set; } = [];
        public PingPackageSettings PingPackageSettings { get; set; }
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
    }

    public enum PackageType
    {
        Ping = 0,
        External = 10
    }
}