﻿using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using CoMon.Authorization.Roles;
using CoMon.Authorization.Users;
using CoMon.MultiTenancy;
using Abp.Localization;
using System;
using CoMon.Groups;
using CoMon.Assets;
using CoMon.Packages;
using CoMon.Statuses;
using CoMon.Images;
using CoMon.Dashboards;

namespace CoMon.EntityFrameworkCore
{
    public class CoMonDbContext : AbpZeroDbContext<Tenant, Role, User, CoMonDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<KPI> KPIs { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Chart> Charts { get; set; }
        public virtual DbSet<Series> Series { get; set; }
        public virtual DbSet<DataPoint> DataPoints { get; set; }
        public virtual DbSet<Dashboard> Dashboards { get; set; }
        public virtual DbSet<DashboardTile> DashboardTiles { get; set; }

        public CoMonDbContext(DbContextOptions<CoMonDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationLanguageText>()
                .Property(p => p.Value)
                .HasMaxLength(100); // any integer that is smaller than 10485760

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                foreach (var property in entityType.GetProperties())
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                        property.SetValueConverter(new UtcDateTimeConverter());
        }
    }
}
