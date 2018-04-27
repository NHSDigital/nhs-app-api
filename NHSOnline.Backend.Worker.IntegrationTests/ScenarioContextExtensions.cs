using NHSOnline.Backend.Worker.IntegrationTests.Worker;
using NHSOnline.Backend.Worker.IntegrationTests.Worker.Models.Patient;
using NHSOnline.Backend.Worker.Mocking;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests
{
    public static class ScenarioContextExtensions
    {
        private const string KeyConnectionToken = "ConnectionToken";
        private const string KeyHttpExceptionExpected = "HttpExceptionExpected";
        private const string KeyHttpException = "HttpException";
        private const string KeyMockingClient = "MockingClient";
        private const string KeyNhsNumber = "NhsNumber";
        private const string KeyNhsNumbers = "NhsNumbers";
        private const string KeyOdsCode = "OdsCode";
        private const string KeySessionDetails = "SessionDetails";
        private const string KeyWorkerClient = "WorkerClient";
        private const string KeyIm1ConnectionRequest = "Im1ConnectionRequest";

        public static string GetConnectionToken(this ScenarioContext context)
        {
            return context.ContainsKey(KeyConnectionToken) ? context.Get<string>(KeyConnectionToken) : null;
        }

        public static ScenarioContext SetConnectionToken(this ScenarioContext context, string connectionToken)
        {
            context.Set(connectionToken, KeyConnectionToken);
            return context;
        }

        public static ScenarioContext SetHttpExceptionExpected(this ScenarioContext context, bool httpExceptionExpected)
        {
            context.Set(httpExceptionExpected, KeyHttpExceptionExpected);
            return ScenarioContext.Current;
        }

        public static bool GetHttpExceptionExpected(this ScenarioContext context)
        {
            return context.ContainsKey(KeyHttpExceptionExpected) && context.Get<bool>(KeyHttpExceptionExpected);
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

        public static Im1ConnectionRequest GetIm1ConnectionRequest(this ScenarioContext context)
        {
            return context.ContainsKey(KeyIm1ConnectionRequest) ? context.Get<Im1ConnectionRequest>(KeyIm1ConnectionRequest) : null;
        }

        public static ScenarioContext SetIm1ConnectionRequest(this ScenarioContext context, Im1ConnectionRequest im1ConnectionRequest)
        {
            context.Set(im1ConnectionRequest, KeyIm1ConnectionRequest);
            return context;
        }

        public static ScenarioContext SetSessionDetails(this ScenarioContext context, string givenName, string familyName)
        {
            var sessionDetails = new UserSessionResponse
            {
                GivenName = givenName,
                FamilyName = familyName
            };

            context.Set(sessionDetails, KeySessionDetails);
            return context;
        }

        public static UserSessionResponse GetSessionDetails(this ScenarioContext context)
        {
            return context.ContainsKey(KeySessionDetails) ? context.Get<UserSessionResponse>(KeySessionDetails) : null;
        }

    }
}

