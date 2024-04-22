using Abp.Domain.Repositories;
using Abp.Threading.Timers;
using CoMon.Packages.Settings;
using CoMon.Statuses;
using Microsoft.Extensions.Logging;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace CoMon.Packages.Workers
{
    public class PingWorker : PackageWorkerBase<PingPackageSettings>
    {

        public PingWorker(AbpAsyncTimer timer, IRepository<Package, long> packageRepository,
            ILogger<PingWorker> logger, IRepository<Status, long> statusRepository)
                : base(timer, packageRepository, logger, statusRepository)
        { }

        protected override PackageType Type => PackageType.Ping;

        protected override async Task<Status> PerformCheck(Package package)
        {
            try
            {
                var pingSender = new Ping();
                var reply = await pingSender.SendPingAsync(package.PingPackageSettings.Host);
                return CreateStatus(package.PingPackageSettings.Host, reply);
            }
            catch (Exception ex)
            {
                logger.LogError("Error while performing ping check for package with id {packageId}: {message}", package.Id, ex.Message);
                return new Status
                {
                    Time = DateTime.UtcNow,
                    Criticality = Criticality.Alert,
                    Messages = ["An error occurred when running the ping check. Check the log for details."]
                };
            }
        }

        private static Status CreateStatus(string host, PingReply reply)
        {
            var pingable = reply.Status == IPStatus.Success;

            return new Status
            {
                Time = DateTime.UtcNow,
                Criticality = pingable ? Criticality.Healthy : Criticality.Alert,
                Messages = pingable ? [$"Successfully pinged {host}."] : [$"Unable to ping {host}."],
                KPIs = pingable ?
                    [
                        new KPI()
                        {
                            Name = "RoundTrip time",
                            Unit = "ms",
                            Value = reply.RoundtripTime
                        }
                    ]
                    : []
            };
        }
    }
}
