using NHSOnline.Backend.Worker.IntegrationTests.Mocking;
using NHSOnline.Backend.Worker.IntegrationTests.Worker;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests
{
    public static class ScenarioContextExtensions
    {
        private const string KeyConnectionToken = "ConnectionToken";
        private const string KeyHttpException = "HttpException";
        private const string KeyMockingClient = "MockingClient";
        private const string KeyNhsNumber = "NhsNumber";
        private const string KeyNhsNumbers = "NhsNumbers";
        private const string KeyOdsCode = "OdsCode";
        private const string KeyWorkerClient = "WorkerClient";

        public static string GetConnectionToken(this ScenarioContext context)
        {
            return context.ContainsKey(KeyConnectionToken) ? context.Get<string>(KeyConnectionToken) : null;
        }

        public static ScenarioContext SetConnectionToken(this ScenarioContext context, string connectionToken)
        {
            context.Set(connectionToken, KeyConnectionToken);
            return context;
        }

        public static NhsoHttpException GetHttpException(this ScenarioContext context)
        {
            return context.ContainsKey(KeyHttpException) ? context.Get<NhsoHttpException>(KeyHttpException) : null;
        }

        public static ScenarioContext SetHttpException(this ScenarioContext context, NhsoHttpException httpException)
        {
            context.Set(httpException, KeyHttpException);
            return context;
        }

        public static string GetNhsNumber(this ScenarioContext context)
        {
            return context.ContainsKey(KeyNhsNumber) ? context.Get<string>(KeyNhsNumber) : null;
        }

        public static ScenarioContext SetNhsNumber(this ScenarioContext context, string nhsNumber)
        {
            context.Set(nhsNumber, KeyNhsNumber);
            return context;
        }

        public static string[] GetNhsNumbers(this ScenarioContext context)
        {
            return context.ContainsKey(KeyNhsNumbers) ? context.Get<string[]>(KeyNhsNumbers) : null;
        }

        public static ScenarioContext SetNhsNumbers(this ScenarioContext context, string[] nhsNumbers)
        {
            context.Set(nhsNumbers, KeyNhsNumbers);
            return context;
        }

        public static MockingClient GetMockingClient(this ScenarioContext context)
        {
            return context.Get<MockingClient>(KeyMockingClient);
        }

        public static ScenarioContext SetMockingClient(this ScenarioContext context, MockingClient client)
        {
            context.Set(client, KeyMockingClient);
            return context;
        }

        public static string GetOdsCode(this ScenarioContext context)
        {
            return context.ContainsKey(KeyOdsCode) ? context.Get<string>(KeyOdsCode) : null;
        }

        public static ScenarioContext SetOdsCode(this ScenarioContext context, string odsCode)
        {
            context.Set(odsCode, KeyOdsCode);
            return context;
        }

        public static WorkerClient GetWorkerClient(this ScenarioContext context)
        {
            return context.Get<WorkerClient>(KeyWorkerClient);
        }

        public static ScenarioContext SetWorkerClient(this ScenarioContext context, WorkerClient client)
        {
            context.Set(client, KeyWorkerClient);
            return context;
        }
    }
}

