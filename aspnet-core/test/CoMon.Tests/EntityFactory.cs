using CoMon.Assets;
using CoMon.Packages;
using CoMon.Statuses;
using System;
using CoMon.Groups;
using CoMon.Images;
using CoMon.Dashboards;
using CoMon.Packages.Settings;

namespace CoMon.Tests
{
    public static class EntityFactory
    {
        public static Group CreateGroup()
        {
            return new Group()
            {
                Name = "Test Group"
            };
        }

        public static Group AddSubGroup(this Group group)
        {
            group.SubGroups.Add(new Group()
            {
                Name = "Test Sub Group"
            });
            return group;
        }

        public static Group AddAsset(this Group group, Asset asset)
        {
            group.Assets.Add(asset);
            return group;
        }

        public static Asset CreateAsset()
        {
            return new Asset()
            {
                Name = "Test Asset",
                Description = "Test Description"
            };
        }

        public static Asset AddImage(this Asset asset)
        {
            asset.Images.Add(new Image()
            {
                MimeType = "image/png",
                Data = [0x00, 0x01, 0x02]
            });
            return asset;
        }

        public static Asset AddPingPackage(this Asset asset, string host = "localhost")
        {
            asset.Packages.Add(new Package()
            {
                Type = PackageType.Ping,
                Guid = Guid.NewGuid(),
                Name = "Test Alert Package",
                PingPackageSettings = new PingPackageSettings()
                {
                    Host = host,
                    CycleSeconds = 60
                }
            });
            return asset;
        }

        public static Asset AddPingPackageWithStatus(this Asset asset, Criticality criticality, int secondsSinceStatus = 0)
        {
            asset.Packages.Add(new Package()
            {
                Type = PackageType.Ping,
                Guid = Guid.NewGuid(),
                Name = "Test Alert Package",
                PingPackageSettings = new PingPackageSettings()
                {
                    Host = "localhost",
                    CycleSeconds = 60
                },
                Statuses = [new Status()
                {
                    Criticality = criticality,
                    Time = DateTime.UtcNow - TimeSpan.FromSeconds(secondsSinceStatus),
                    KPIs = [new KPI()
                    {
                        Name = "Test KPI",
                        Unit = "Test Unit",
                        Value = 5
                    }]
                }]
            });
            return asset;
        }

        public static Dashboard CreateDashboard()
        {
            return new Dashboard()
            {
                Name = "Test Dashboard"
            };
        }

        public static Dashboard AddTile(this Dashboard dashboard, DashboardTile tile)
        {
            dashboard.Tiles.Add(tile);
            return dashboard;
        }

        public static DashboardTile CreateTile(DashboardTileType type, long itemId, int sortIndex)
        {
            return new DashboardTile()
            {
                ItemType = type,
                ItemId = itemId,
                SortIndex = sortIndex
            };
        }
    }
}
