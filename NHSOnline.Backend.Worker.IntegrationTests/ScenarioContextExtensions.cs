using NHSOnline.Backend.Worker.IntegrationTests.Mocking;
using NHSOnline.Backend.Worker.IntegrationTests.Worker;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests
{
    public static class ScenarioContextExtensions
    {
        private const string KeyMockingClient = "MockingClient";
        private const string KeyWorkerClient = "WorkerClient";

        public static MockingClient MockingClient(this ScenarioContext context, MockingClient mockingClient = null)
        {
            const string key = KeyMockingClient;
            AddIfRequired(key, mockingClient, context);
            return context.ContainsKey(key) ? context.Get<MockingClient>(key) : null;
        }

        public static WorkerClient WorkerClient(this ScenarioContext context, WorkerClient workerClient = null)
        {
            const string key = KeyWorkerClient;
            AddIfRequired(key, workerClient, context);
            return context.ContainsKey(key) ? context.Get<WorkerClient>(key) : null;
        }

        private static void AddIfRequired(string key, object item, ScenarioContext context) {
            if (item != null && !context.ContainsKey(key))
            {
                context.Add(key, item);
            }
        }
    }
}

