using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public interface IEventHubClient
    {
        EventHubs EventHub { get; }

        bool PidAllowed { get; }

        Task WriteToEventHub(string eventLog);
    }
}