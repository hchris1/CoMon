using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using CoMon.Statuses;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoMon.Notifications
{
    public class CoMonHub : Hub, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }
        protected IHubContext<CoMonHub> HubContext { get; set; }

        public ILogger Logger { get; set; }

        public CoMonHub(IHubContext<CoMonHub> hubContext)
        {
            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
            HubContext = hubContext;
        }

        public async Task SendStatusUpdate(Status status)
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var jsonString = JsonSerializer.Serialize(status, options);
            await HubContext.Clients.All.SendAsync("CoMon.Status.Update", jsonString);
        }

        public async Task SendStatusChange(Status status, Criticality? previousCriticality)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var data = new Dictionary<string, object>
            {
                ["id"] = status.Id,
                ["time"] = status.Time,
                ["previousCriticality"] = previousCriticality,
                ["criticality"] = status.Criticality,
                ["packageId"] = status.Package.Id,
                ["packageName"] = status.Package.Name,
                ["assetId"] = status.Package.Asset.Id,
                ["assetName"] = status.Package.Asset.Name
            };

            var jsonString = JsonSerializer.Serialize(data, options);
            await HubContext.Clients.All.SendAsync("CoMon.Status.Change", jsonString);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Logger.Debug("A client connected to CoMonHub: " + Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            Logger.Debug("A client disconnected from CoMonHub: " + Context.ConnectionId);
        }
    }
}
