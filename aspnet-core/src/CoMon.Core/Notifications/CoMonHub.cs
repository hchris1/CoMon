using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoMon.Notifications
{
    public class CoMonHub(IHubContext<CoMonHub> hubContext) : Hub, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; } = NullAbpSession.Instance;
        public IHubContext<CoMonHub> HubContext { get; set; } = hubContext;
        public ILogger Logger { get; set; } = NullLogger.Instance;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task SendStatusUpdate(StatusUpdateDto update)
        {
            var jsonString = JsonSerializer.Serialize(update, _jsonSerializerOptions);
            await HubContext.Clients.All.SendAsync("CoMon.Status.Update", jsonString);
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
