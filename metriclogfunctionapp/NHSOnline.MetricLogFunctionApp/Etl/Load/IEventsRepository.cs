using System.Threading.Tasks;

namespace NHSOnline.MetricLogFunctionApp.Etl.Load
{
    public interface IEventsRepository
    {
        public Task CallStoredProcedure(string call, params object[] parameters);
    }
}