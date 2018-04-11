using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NHSOnline.Backend.Worker.IntegrationTests.Worker;
using NHSOnline.Backend.Worker.IntegrationTests.Worker.Models.Patient;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features.Emis.Im1Connection.Verification
{
    [Binding]
    internal class PatientVerificationSteps
    {
        public const string DefaultOdsCode = "E87649";
        public const string DefaultConnectionToken = "bce74b97-4296-414a-a4f5-0f1bf5732ba6";
        public const string GetIm1ConnectionResult = "GetIm1ConnectionResult";
        private const AssociationType DefaultAssociationType = AssociationType.Self;

        private readonly ScenarioContext _context;
        public PatientVerificationSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"I have an IM1 Connection Token that does not exist")]
        public async Task GivenIHaveAnImConnectionTokenThatDoesNotExist()
        {
            const string nonExistingConnectionToken = "0d135b66-a8b0-46b2-b437-cfe75edc773d";
            const string odsCode = DefaultOdsCode;
            const string endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D";

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(SessionConfigurator
                .ForRequest(nonExistingConnectionToken, odsCode, endUserSessionId)
                .RespondWithUserNotRegistered());

            _context
                .SetConnectionToken(nonExistingConnectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"I have an IM1 Connection Token that is in an invalid format")]
        public void GivenIHaveAnIm1ConnectionTokenThatIsInAnInvalidFormat()
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
        public void GivenIHaveAnOdsCodeNotInExpectedFormat()
        {
            _context
                .SetConnectionToken(DefaultConnectionToken)
                .SetOdsCode("£$*&");
        }

        [Given(@"I have an ODS Code that does not exists")]
        public void GivenIHaveAnOdsCodeThatDoesNotExists()
        {
            _context
                .SetConnectionToken(DefaultConnectionToken)
                .SetOdsCode("E99999");
        }

        [Given(@"I have no ODS Code")]
        public void GivenIHaveNoOdsCode()
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
            const string title = "Mr";
            const string firstName = "Eduardo";
            const string surname = "Crouch";
            const string endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D";
            const string sessionId = "h3pYG9By2tVTqcvPvpw3DL";
            const string userPatientLinkToken = "5d4p6ZExhi97mmerMrtD5p";

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(SessionConfigurator
                .ForRequest(connectionToken, odsCode, endUserSessionId)
                .RespondWithSuccess(sessionId, title, firstName, surname, userPatientLinkToken, odsCode, AssociationType.Self));

            await PostMapping(DemographicsConfigurator
                .ForRequest(endUserSessionId, sessionId, userPatientLinkToken)
                .RespondWithSuccess(title, firstName, surname, new[] { PatientIdentifier.NHSNumber(nhsNumber), }));

            _context
                .SetNhsNumber(nhsNumber)
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"I have valid credentials for a patient with multiple NHS Numbers")]
        public async Task GivenIHaveValidCredentialsForAPatientWithMultipleNhsNumbers()
        {
            const string connectionToken = "fe81f191-b016-466e-aeb2-64f08f2330a4";
            const string odsCode = DefaultOdsCode;
            const string title = "Miss";
            const string firstName = "Alexia";
            const string surname = "Scott";
            const string endUserSessionId = "9RFDWiqTO8zBWrp2p8s4K7";
            const string sessionId = "xkWiivK1WBAkxIN9CDrGyy";
            const string userPatientLinkToken = "KxLiDl5nRS60DzIlrKoFSl";
            var nhsNumbers = new[] { "NHS_number1", "NHS_number2" };

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(SessionConfigurator
                .ForRequest(connectionToken, odsCode, endUserSessionId)
                .RespondWithSuccess(sessionId, title, firstName, surname, userPatientLinkToken, odsCode, AssociationType.Self));

            await PostMapping(DemographicsConfigurator
                .ForRequest(endUserSessionId, sessionId, userPatientLinkToken)
                .RespondWithSuccess(title, firstName, surname, nhsNumbers.Select(PatientIdentifier.NHSNumber)));

            _context
                .SetNhsNumbers(nhsNumbers)
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"I have valid credentials for a patient with no NHS Number")]
        public async Task GivenIHaveValidCredentialsForAPatientWithNoNhsNumber()
        {
            const string connectionToken = "e69ddbd4-2d89-43b7-a252-06dba3558f9f";
            const string odsCode = DefaultOdsCode;
            const string title = "Mr";
            const string firstName = "Rajan";
            const string surname = "Liu";
            const string endUserSessionId = "igOhJWsZ6GOBjaZU5PdR37";
            const string sessionId = "ALtNiTSBVk7VwCe1s4L1mz";
            const string userPatientLinkToken = "vOXLnnw7QLQoghDyqTd1Sa";

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(SessionConfigurator
                .ForRequest(connectionToken, odsCode, endUserSessionId)
                .RespondWithSuccess(sessionId, title, firstName, surname, userPatientLinkToken, odsCode, AssociationType.Self));

            await PostMapping(DemographicsConfigurator
                .ForRequest(endUserSessionId, sessionId, userPatientLinkToken)
                .RespondWithSuccess(title, firstName, surname, Enumerable.Empty<PatientIdentifier>()));

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
                    .GetIm1Connection(connectionToken, odsCode);

                _context.Add(GetIm1ConnectionResult, result);
            }
            catch (NhsoHttpException httpException)
            {

                Console.WriteLine(httpException);

                _context.SetHttpException(httpException);
            }
        }

        [Then(@"I receive the expected NHS Number")]
        public void ThenIReceiveTheExpectedNhsNumber()
        {
            var result = _context.Get<Im1ConnectionResponse>(GetIm1ConnectionResult);
            var nhsNumber = _context.GetNhsNumber();
            var connectionToken = _context.GetConnectionToken();

            result.Should().NotBeNull();
            result.NhsNumbers.First().NhsNumber.Should().Be(nhsNumber);
            result.ConnectionToken.Should().Be(connectionToken);
        }

        [Then(@"I receive the expected NHS Numbers")]
        public void ThenIReceiveTheExpectedNhsNumbers()
        {
            var result = _context.Get<Im1ConnectionResponse>(GetIm1ConnectionResult);
            var nhsNumbers = _context.GetNhsNumbers();
            var connectionToken = _context.GetConnectionToken();

            result.Should().NotBeNull();
            result.NhsNumbers.Select(x => x.NhsNumber).Should().BeEquivalentTo(nhsNumbers);
            result.ConnectionToken.Should().Be(connectionToken);
        }

        [Then(@"I receive no NHS Number")]
        public void ThenIReceiveNoNhsNumber()
        {
            var result = _context.Get<Im1ConnectionResponse>(GetIm1ConnectionResult);

            result.NhsNumbers.Should().BeEmpty();
        }

        //private async Task MockEmisDelay(TimeSpan delay)
        //{
        //    await _context.GetMockingClient().PostMappingAsync(SessionConfigurator.CreateEndUserSessionMappingWithTimout(delay));
        //}

        //private async Task MockSuccessfulIm1Connection(string nhsNumber, string connectionToken, string odsCode)
        //{
        //    var nhsNumbers = nhsNumber == null ? new string[0] : new[] { nhsNumber };
        //    await MockSuccessfulIm1Connection(nhsNumbers, connectionToken, odsCode);
        //}

        //private async Task MockSuccessfulIm1Connection(string[] nhsNumbers, string connectionToken, string odsCode)
        //{
        //    const int statusCodeCreated = (int)HttpStatusCode.Created;
        //    const int statusCodeOk = (int)HttpStatusCode.OK;
        //    const string endUserSessionId = DefaultEndUserSessionId;
        //    const string linkToken = DefaultLinkToken;
        //    const string sessionId = DefaultSessionId;

        //    await _context.GetMockingClient().PostMappingAsync(SessionConfigurator.CreateEndUserSessionMapping(statusCodeCreated, endUserSessionId));
        //    await _context.GetMockingClient().PostMappingAsync(SessionConfigurator.CreateSessionsMapping(statusCodeCreated, connectionToken, odsCode, sessionId, linkToken, DefaultAssociationType, endUserSessionId));
        //    await _context.GetMockingClient().PostMappingAsync(DemographicsConfigurator.CreateDemographicsMapping(statusCodeOk, linkToken, sessionId, endUserSessionId, nhsNumbers));
        //}

        private async Task PostMapping(Mapping mapping)
        {
            await _context.GetMockingClient()
                .PostMappingAsync(mapping);
        }
    }
}
