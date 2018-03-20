using System.Threading.Tasks;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking;
using NHSOnline.Backend.Worker.IntegrationTests.Worker;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests
{
    [Binding]
    public class GlobalHooks
    {
        private static readonly MockingClient MockingClient = new MockingClient();
        private static readonly WorkerClient WorkerClient = new WorkerClient();

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            await MockingClient.ResetMappings();
            ScenarioContext.Current.WorkerClient(WorkerClient);
            ScenarioContext.Current.MockingClient(MockingClient);
        }
    }
}
