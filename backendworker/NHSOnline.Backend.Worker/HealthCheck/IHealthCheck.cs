using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.HealthCheck
{
    public interface IHealthCheck
    {
        Task<HealthCheck.Result> Execute();
    }
}