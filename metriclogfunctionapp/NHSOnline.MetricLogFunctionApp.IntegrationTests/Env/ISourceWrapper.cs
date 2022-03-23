using System.Threading.Tasks;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env
{
    internal interface ISourceWrapper
    {
        Task Initialise(TestLogs logs);
    }
}