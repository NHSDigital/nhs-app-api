using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Mocking.Models;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features.Shared
{
    [Binding]
    public class CommonSteps
    {
        private readonly ScenarioContext _context;

        public CommonSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"EMIS is unavailable")]
        public async Task GivenEmisIsUnavailable()
        {
            const string connectionToken = "f6ca8e0c-dd67-4863-ba9e-3d34bfe930d0";
            const string odsCode = "A29928";

            _context
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithServiceUnavailable());
        }

        [Then(@"I receive (?:a|an) ""(.*)"" error")]
        public void ThenIReceiveAnMessage(HttpStatusCode expectedStatusCode)
        {
            var exception = _context.GetHttpException();
            exception.Should().NotBeNull("An exception was expected but was not returned within the expected time limit.");
            exception.StatusCode.Should().Be(expectedStatusCode);
        }

        private async Task PostMapping(Mapping mapping)
        {
            await _context.GetMockingClient()
                .PostMappingAsync(mapping);
        }
    }
}
