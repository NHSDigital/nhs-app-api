using System.Threading.Tasks;
using NHSOnline.Backend.Worker.IntegrationTests.Worker;
using NHSOnline.Backend.Worker.Mocking;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests
{
    [Binding]
    public class GlobalHooks
    {
        private static readonly MockingClient MockingClient = new MockingClient(Configuration.ToMockingConfiguration());
        private static readonly WorkerClient WorkerClient = new WorkerClient();

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            await MockingClient.ResetMappings();
            ScenarioContext.Current
                .SetWorkerClient(WorkerClient)
                .SetMockingClient(MockingClient);
        }
    }
}
