using Abp.Events.Bus.Entities;
using CoMon.Statuses;
using System.Threading.Tasks;

namespace CoMon.Notifications
{
    public interface INotificationService
    {
        Task HandleEventAsync(EntityCreatedEventData<Status> eventData);
    }
}