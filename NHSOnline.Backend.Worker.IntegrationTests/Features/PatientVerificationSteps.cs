using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Nhso.Models.Patient;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features
{
    [Binding]
    internal class PatientVerificationSteps
    {
        private const string ConnectionToken = "ConnectionToken";
        private const string NhsNumber = "NhsNumber";
        private const string GetIm1ConnectionResult = "GetIm1ConnectionResult";
        private const string OdsCode = "OdsCode";
        private readonly ScenarioContext _context;

        public PatientVerificationSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"I have valid credentials for a patient with one NHS Number")]
        public async Task GivenIHaveValidCredentialsForAPatientWithOneNhsNumber()
        {
            const int statusCodeCreated = (int)HttpStatusCode.Created;
            const int statusCodeOk = (int)HttpStatusCode.OK;
            const string endUserSessionId = "bar";
            const string connectionToken = "token";
            const string odsCode = "E87649";
            const string linkToken = "link_token";
            const string sessionId = "session_id";
            const string nhsNumber = "NHS_number";
            const AssociationType associationType = AssociationType.Self;

            await _context.MockingClient().PostMappingAsync(SessionConfigurator.CreateEndUserSessionMapping(statusCodeCreated, endUserSessionId));
            await _context.MockingClient().PostMappingAsync(SessionConfigurator.CreateSessionsMapping(statusCodeCreated, connectionToken, odsCode, sessionId, linkToken, associationType, endUserSessionId));
            await _context.MockingClient().PostMappingAsync(DemographicsConfigurator.CreateDemographicsMapping(statusCodeOk, linkToken, sessionId, endUserSessionId, nhsNumber));

            _context.Add(NhsNumber, nhsNumber);
            _context.Add(ConnectionToken, connectionToken);
            _context.Add(OdsCode, odsCode);
        }

        [When(@"I verify patient data")]
        public async Task WhenIVerifyPatientData()
        {
            var connectionToken = _context.Get<string>(ConnectionToken);
            var odsCode = _context.Get<string>(OdsCode);

            var result = await _context.WorkerClient().Patient.GetIm1Connection(connectionToken, odsCode);
            _context.Add(GetIm1ConnectionResult, result);
        }

        [Then(@"I receive the expected NHS Number")]
        public void ThenIReceiveTheExpectedNhsNumber()
        {
            var result = _context.Get<Im1ConnectionResponse>(GetIm1ConnectionResult);
            var nhsNumber = _context.Get<string>(NhsNumber);
            var connectionToken = _context.Get<string>(ConnectionToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(nhsNumber, result.NhsNumbers.First().NhsNumber);
            Assert.AreEqual(connectionToken, result.ConnectionToken);
        }
    }
}
