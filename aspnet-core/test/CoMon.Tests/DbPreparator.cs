using CoMon.Assets;
using CoMon.EntityFrameworkCore;
using CoMon.Packages.Settings;
using CoMon.Packages;
using CoMon.Statuses;
using System;
using CoMon.Groups;
using CoMon.Images;

namespace CoMon.Tests
{
    public static class DbPreparator
    {
        public static void CreateGroupWithoutAssets(CoMonDbContext context)
        {
            context.Groups.Add(
                new Group()
                {
                    Id = 1,
                    Name = "Test Group",
                });
        }

        public static void CreateAssetWithImage(CoMonDbContext context)
        {
            context.Assets.Add(
                new Asset()
                {
                    Id = 1,
                    Name = "Test Asset",
                    Description = "Test Description",
                    Images = [new Image()
                        {
                            Id = 1,
                            MimeType = "image/png",
                            Data = [0x00, 0x01, 0x02]
                        }]
                });
        }

        public static void CreateAssetWithoutPackages(CoMonDbContext context)
        {
            context.Assets.Add(
                new Asset()
                {
                    Id = 1,
                    Name = "Test Asset",
                    Description = "Test Description",
                    Packages = []
                });
        }

        public static void CreateAssetWithPingPackage(CoMonDbContext context, bool hasStatus)
        {
            context.Assets.Add(
                new Asset()
                {
                    Id = 1,
                    Name = "Test Asset",
                    Description = "Test Description",
                    Packages = [new Package() {
                        Type = PackageType.Ping,
                        Guid = Guid.NewGuid(),
                        Name = "Test Ping Package",
                        PingPackageSettings = new PingPackageSettings() {
                            CycleSeconds = 30,
                            Host = "localhost"
                        },
                        Statuses = hasStatus ? [new Status() {
                            Criticality = Criticality.Healthy,
                            Time = DateTime.UtcNow
                        }] : []
                    }]
                });
        }
    }
}
