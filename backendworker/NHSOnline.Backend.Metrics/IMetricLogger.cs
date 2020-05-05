using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics
{
    public interface IMetricLogger
    {
        Task Login();

        Task UpliftStarted();
    }
}
