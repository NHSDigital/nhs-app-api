using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics
{
    public interface IAnonymousMetricLogger
    {
        Task AppointmentBookResult(AppointmentMetricData data);

        Task AppointmentCancelResult(AppointmentMetricData data);
    }
}