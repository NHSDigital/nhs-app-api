using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Nhso.Models.Patient;
using NHSOnline.Backend.Worker.IntegrationTests.Worker;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features
{
    [Binding]
    internal class PatientVerificationSteps
    {
        private const string MockingClientContextKey = "MOckingClient";
        public const string DefaultOdsCode = "E87649";
        public const string DefaultConnectionToken = "bce74b97-4296-414a-a4f5-0f1bf5732ba6";
        public const string GetIm1ConnectionResult = "GetIm1ConnectionResult";
        private const string DefaultSessionId = "session_id";
        private const string DefaultLinkToken = "link_token";
        private const string DefaultEndUserSessionId = "bar";
        private const AssociationType DefaultAssociationType = AssociationType.Self;
        private readonly TimeSpan TestLimit = TimeSpan.FromSeconds(10);

        private readonly ScenarioContext _context;
        public PatientVerificationSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"I have an IM1 Connection Token that does not exist")]
        public async Task GivenIHaveAnIMConnectionTokenThatDoesNotExist()
        {
            const string nonExistingConnectionToken = "0d135b66-a8b0-46b2-b437-cfe75edc773d";

            // Setup the mock server to return using the default connection token not the missing one
            await MockSuccessfulIm1Connection((string) null, DefaultConnectionToken, DefaultOdsCode);
            await _context
                .GetMockingClient()
                .PostMappingAsync(
                    SessionConfigurator.CreateSessionsMapping(
                        500,
                        nonExistingConnectionToken,
                        DefaultOdsCode,
                        DefaultSessionId,
                        DefaultLinkToken,
                        DefaultAssociationType,
                        DefaultEndUserSessionId
                    )
                );

            _context
                .SetConnectionToken(nonExistingConnectionToken)
                .SetOdsCode(DefaultOdsCode);
        }

        [Given(@"I have an IM1 Connection Token that is in an invalid format")]
        public void GivenIHaveAnIM1ConnectionTokenThatIsInAnInvalidFormat()
        {
            _context
                .SetConnectionToken("token")
                .SetOdsCode(DefaultOdsCode);
        }

        [Given(@"I have no IM1 Connection Token")]
        public void GivenIHaveNoIm1ConnectionToken()
        {
            _context
                .SetConnectionToken(null)
                .SetOdsCode(DefaultOdsCode);
        }

        [Given(@"I have an ODS Code not in expected format")]
        public void GivenIHaveAnODSCodeNotInExpectedFormat()
        {
            _context
                .SetConnectionToken(DefaultConnectionToken)
                .SetOdsCode("£$*&");
        }

        [Given(@"I have an ODS Code that does not exists")]
        public void GivenIHaveAnODSCodeThatDoesNotExists()
        {
            _context
                .SetConnectionToken(DefaultConnectionToken)
                .SetOdsCode("E99999");
        }

        [Given(@"I have no ODS Code")]
        public void GivenIHaveNoODSCode()
        {
            _context
                .SetConnectionToken(DefaultConnectionToken)
                .SetOdsCode(null);
        }

        [Given(@"I have valid credentials for a patient with one NHS Number")]
        public async Task GivenIHaveValidCredentialsForAPatientWithOneNhsNumber()
        {
            const string connectionToken = DefaultConnectionToken;
            const string nhsNumber = "NHS_number";
            const string odsCode = DefaultOdsCode;

            if (_context.TryGetValue("TimeoutDelay", out TimeSpan delay))
            {
                await MockEmisDelay(delay);

            }
            else
            {
                await MockSuccessfulIm1Connection(nhsNumber, connectionToken, odsCode);
            }

            _context
                .SetNhsNumber(nhsNumber)
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"I have valid credentials for a patient with multiple NHS Numbers")]
        public async Task GivenIHaveValidCredentialsForAPatientWithMultipleNHSNumbers()
        {
            const string connectionToken = DefaultConnectionToken;
            const string odsCode = DefaultOdsCode;
            var nhsNumbers = new[] { "NHS_number1", "NHS_number2" };

            await MockSuccessfulIm1Connection(nhsNumbers, connectionToken, odsCode);

            _context
                .SetNhsNumbers(nhsNumbers)
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"I have valid credentials for a patient with no NHS Number")]
        public async Task GivenIHaveValidCredentialsForAPatientWithNoNHSNumber()
        {
            const string connectionToken = DefaultConnectionToken;
            const string odsCode = DefaultOdsCode;
            const string nhsNumber = null;

            await MockSuccessfulIm1Connection(nhsNumber, connectionToken, odsCode);

            _context
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [When(@"I verify patient data")]
        public async Task WhenIVerifyPatientData()
        {
            var connectionToken = _context.GetConnectionToken();
            var odsCode = _context.GetOdsCode();

            try
            {
                var result = await _context
                    .GetWorkerClient()
                    .GetIm1Connection(connectionToken, odsCode)
                    .WithTimeout(TestLimit);

                _context.Add(GetIm1ConnectionResult, result);
            }
            catch (NhsoHttpException httpException)
            {
                _context.SetHttpException(httpException);
            }
        }

        [Then(@"I receive the expected NHS Number")]
        public void ThenIReceiveTheExpectedNhsNumber()
        {
            var result = _context.Get<Im1ConnectionResponse>(GetIm1ConnectionResult);
            var nhsNumber = _context.GetNhsNumber();
            var connectionToken = _context.GetConnectionToken();

            Assert.IsNotNull(result);
            Assert.AreEqual(nhsNumber, result.NhsNumbers.First().NhsNumber);
            Assert.AreEqual(connectionToken, result.ConnectionToken);
        }

        [Then(@"I receive the expected NHS Numbers")]
        public void ThenIReceiveTheExpectedNhsNumbers()
        {
            var result = _context.Get<Im1ConnectionResponse>(GetIm1ConnectionResult);
            var nhsNumbers = _context.GetNhsNumbers();
            var connectionToken = _context.GetConnectionToken();

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(nhsNumbers, result.NhsNumbers.Select(x => x.NhsNumber).ToArray());
            Assert.AreEqual(connectionToken, result.ConnectionToken);
        }

        [Then(@"I receive no NHS Number")]
        public void ThenIReceiveNoNhsNumber()
        {
            var result = _context.Get<Im1ConnectionResponse>(GetIm1ConnectionResult);

            var actualNhsNumbers = result?.NhsNumbers;
            actualNhsNumbers.Should().BeEmpty();
        }

        private async Task MockEmisDelay(TimeSpan delay)
        {
            await _context.GetMockingClient().PostMappingAsync(SessionConfigurator.CreateEndUserSessionMappingWithTimout(delay));
        }

        private async Task MockSuccessfulIm1Connection(string nhsNumber, string connectionToken, string odsCode)
        {
            var nhsNumbers = nhsNumber == null ? new string[0] : new[] { nhsNumber };
            await MockSuccessfulIm1Connection(nhsNumbers, connectionToken, odsCode);
        }

        private async Task MockSuccessfulIm1Connection(string[] nhsNumbers, string connectionToken, string odsCode)
        {
            const int statusCodeCreated = (int)HttpStatusCode.Created;
            const int statusCodeOk = (int)HttpStatusCode.OK;
            const string endUserSessionId = DefaultEndUserSessionId;
            const string linkToken = DefaultLinkToken;
            const string sessionId = DefaultSessionId;

            await _context.GetMockingClient().PostMappingAsync(SessionConfigurator.CreateEndUserSessionMapping(statusCodeCreated, endUserSessionId));
            await _context.GetMockingClient().PostMappingAsync(SessionConfigurator.CreateSessionsMapping(statusCodeCreated, connectionToken, odsCode, sessionId, linkToken, DefaultAssociationType, endUserSessionId));
            await _context.GetMockingClient().PostMappingAsync(DemographicsConfigurator.CreateDemographicsMapping(statusCodeOk, linkToken, sessionId, endUserSessionId, nhsNumbers));
        }
    }
}
