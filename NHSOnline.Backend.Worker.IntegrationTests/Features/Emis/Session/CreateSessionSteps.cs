using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NHSOnline.Backend.Worker.IntegrationTests.Worker;
using NHSOnline.Backend.Worker.IntegrationTests.Worker.Models.Patient;
using NHSOnline.Backend.Worker.IntegrationTests.Worker.Models.User;
using NHSOnline.Backend.Worker.Mocking.CID;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features.Emis.Im1Connection.Verification
{
    [Binding]
    internal class CreateSesionSteps
    {
        private const string DefaultOdsCode = "E87649";
        private const string DefaultConnectionToken = "bce74b97-4296-414a-a4f5-0f1bf5732ba6";
        private const AssociationType DefaultAssociationType = AssociationType.Self;
        private const string GetUserSessionResult = "GetUserSessionResult";
        private const string ConnectionToken = "0d135b66-a8b0-46b2-b437-cfe75edc773d";
        private const string AuthCode = "uss.UHLq4ghr4wsANlw5lMdUPFRGji4xlmPSETNewHxUpW0.4dff5848-0cc8-47a1-8eb1-7657b5e9e403.8d4c0a21-6483-4a52-9d47-6bcd737c634e";
        private const string CodeVerifier = "xmoKFiYSK6APIDwc7cULOskbmkWD3vD2Map5lIQDdVU";
        private const string AccessToken = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3WGlNdHVGSHN0MVdtYUppSXdJYVZJZlJOazQ5SzlTNXlWa0thMkZZZHA0In0.eyJqdGkiOiIwNjBlZjM4Yy00YmRlLTQ1ZjgtYjExMy1iOGYzZjVjYWJjOGQiLCJleHAiOjE1MjM5NTg5MTYsIm5iZiI6MCwiaWF0IjoxNTIzOTU4NjE2LCJpc3MiOiJodHRwczovL2tleWNsb2FrLmRldjEuc2lnbmluLm5ocy51ay9jaWNhdXRoL3JlYWxtcy9OSFMiLCJhdWQiOiJuaHMtb25saW5lLXBvYyIsInN1YiI6IjlmZmFhMmNiLTM3MTQtNDMwOS1hZDIyLTlkNGY2YmYwZjUzMSIsInR5cCI6IkJlYXJlciIsImF6cCI6Im5ocy1vbmxpbmUtcG9jIiwiYXV0aF90aW1lIjoxNTIzOTU4NTkyLCJzZXNzaW9uX3N0YXRlIjoiYTRmYmYwN2EtNGM3MS00MTdjLWE2OTYtYmUxNjQ3MDIwOGM0IiwiYWNyIjoiMSIsImFsbG93ZWQtb3JpZ2lucyI6W10sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJ1bWFfYXV0aG9yaXphdGlvbiJdfSwicmVzb3VyY2VfYWNjZXNzIjp7InJlYWxtLW1hbmFnZW1lbnQiOnsicm9sZXMiOlsidmlldy1yZWFsbSIsInZpZXctaWRlbnRpdHktcHJvdmlkZXJzIiwibWFuYWdlLWlkZW50aXR5LXByb3ZpZGVycyIsImltcGVyc29uYXRpb24iLCJyZWFsbS1hZG1pbiIsImNyZWF0ZS1jbGllbnQiLCJtYW5hZ2UtdXNlcnMiLCJxdWVyeS1yZWFsbXMiLCJ2aWV3LWF1dGhvcml6YXRpb24iLCJxdWVyeS1jbGllbnRzIiwicXVlcnktdXNlcnMiLCJtYW5hZ2UtZXZlbnRzIiwibWFuYWdlLXJlYWxtIiwidmlldy1ldmVudHMiLCJ2aWV3LXVzZXJzIiwidmlldy1jbGllbnRzIiwibWFuYWdlLWF1dGhvcml6YXRpb24iLCJtYW5hZ2UtY2xpZW50cyIsInF1ZXJ5LWdyb3VwcyJdfSwiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwibmFtZSI6IlJlYWxtMSBBZG1pbiIsInByZWZlcnJlZF91c2VybmFtZSI6InJlYWxtYWRtaW5AZ21haWwuY29tIiwiZ2l2ZW5fbmFtZSI6IlJlYWxtMSIsImZhbWlseV9uYW1lIjoiQWRtaW4iLCJlbWFpbCI6InJlYWxtYWRtaW5AZ21haWwuY29tIn0.D2nSVJbZ7M2JZosiC6z-HXx7-Rg1n7w7CCKvWtBzErJVDIedvS5y6syxQnJbtl0yITYM4qP-gN0Ji13qnwu0wjy-NorXvG7BOB5wl2SXekaaphXjv9e6NshQ5SEhyV1hMzfPRqLkZbpETjEOdPiMziG6k8sZCpast3c3diKb96dxjVIOhPayf2P9Z75b-qnegFuV1LkD9mIkGDyA7t5givfouskPSr09EKyxHf_m7kjPipy39cKODgcbsyYpwqAmHYaHJGsqIZYDPTCjvzmkrZOQlGJ_sXAVmxrZY8psUZ7MKeFd4l9xwvfi4N-3FFT5D4_tJq0Yp3RW5Bs3JVc1ig";
        private const string BearerToken = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3WGlNdHVGSHN0MVdtYUppSXdJYVZJZlJOazQ5SzlTNXlWa0thMkZZZHA0In0.eyJqdGkiOiJmOGU2YTM0ZC0zODBmLTQzMmYtYTI1Yy0xNDMxYTNhOTZhMmYiLCJleHAiOjE1MjM5NjE0NzMsIm5iZiI6MCwiaWF0IjoxNTIzOTYxMTczLCJpc3MiOiJodHRwczovL2tleWNsb2FrLmRldjEuc2lnbmluLm5ocy51ay9jaWNhdXRoL3JlYWxtcy9OSFMiLCJhdWQiOiJuaHMtb25saW5lLXBvYyIsInN1YiI6IjlmZmFhMmNiLTM3MTQtNDMwOS1hZDIyLTlkNGY2YmYwZjUzMSIsInR5cCI6IkJlYXJlciIsImF6cCI6Im5ocy1vbmxpbmUtcG9jIiwiYXV0aF90aW1lIjoxNTIzOTYxMTUwLCJzZXNzaW9uX3N0YXRlIjoiNGRmZjU4NDgtMGNjOC00N2ExLThlYjEtNzY1N2I1ZTllNDAzIiwiYWNyIjoiMSIsImFsbG93ZWQtb3JpZ2lucyI6W10sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJ1bWFfYXV0aG9yaXphdGlvbiJdfSwicmVzb3VyY2VfYWNjZXNzIjp7InJlYWxtLW1hbmFnZW1lbnQiOnsicm9sZXMiOlsidmlldy1yZWFsbSIsInZpZXctaWRlbnRpdHktcHJvdmlkZXJzIiwibWFuYWdlLWlkZW50aXR5LXByb3ZpZGVycyIsImltcGVyc29uYXRpb24iLCJyZWFsbS1hZG1pbiIsImNyZWF0ZS1jbGllbnQiLCJtYW5hZ2UtdXNlcnMiLCJxdWVyeS1yZWFsbXMiLCJ2aWV3LWF1dGhvcml6YXRpb24iLCJxdWVyeS1jbGllbnRzIiwicXVlcnktdXNlcnMiLCJtYW5hZ2UtZXZlbnRzIiwibWFuYWdlLXJlYWxtIiwidmlldy1ldmVudHMiLCJ2aWV3LXVzZXJzIiwidmlldy1jbGllbnRzIiwibWFuYWdlLWF1dGhvcml6YXRpb24iLCJtYW5hZ2UtY2xpZW50cyIsInF1ZXJ5LWdyb3VwcyJdfSwiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwibmFtZSI6IlJlYWxtMSBBZG1pbiIsInByZWZlcnJlZF91c2VybmFtZSI6InJlYWxtYWRtaW5AZ21haWwuY29tIiwiZ2l2ZW5fbmFtZSI6IlJlYWxtMSIsImZhbWlseV9uYW1lIjoiQWRtaW4iLCJlbWFpbCI6InJlYWxtYWRtaW5AZ21haWwuY29tIn0.YpV1RUKXkSFm4y4fcDEANzgebK8k2E32aFO1Za7vmkDfTRKQG1yWMZuT-IYZQdt5X1EPd7j1IE3nz-j2sFWjuPp2on78JIPgS86gV8NmhU-f7dK_wrFc0_o0H_yrOyPiieP6MTISYdUYUp74mUZuKv2I8B53oOJ-n33GNXDR94u2bMoVQglAwSXH1xAgc6zszl0LZNu6p1qeZjX6LZPJdJTFUW540tX12TGZmDM9X4dbyYMq1jjF-V6k39KJtgCfhCCV16MH-72N9Cx1RpF-43hpw4WOjpooWDxjBKn2PGvSg8z6LpgB7O-O0-4n28NnQ8hAA5JQjcCZQfJ87L_1Iw";
        private const string endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D";
        private readonly ScenarioContext _context;

        public CreateSesionSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"I have a valid authCode and codeVerifier for a patient")]
        public async Task GivenIHaveAValidAuthCodeAndCodeVerifierForAPatient()
        {
            const string title = "Mr";
            const string firstName = "Eduardo";
            const string surname = "Crouch";
            const string sessionId = "h3pYG9By2tVTqcvPvpw3DL";
            const string userPatientLinkToken = "5d4p6ZExhi97mmerMrtD5p";

        await PostMapping(TokenConfigurator
                .ForRequest(AuthCode, CodeVerifier)
                .RespondWithSuccess(AccessToken));

            await PostMapping(UserProfileConfigurator
                .ForRequest(BearerToken)
                .RespondWithSuccess(DefaultOdsCode, ConnectionToken));

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(SessionConfigurator
                .ForRequest(ConnectionToken, DefaultOdsCode, endUserSessionId)
                .RespondWithSuccess(sessionId, title, firstName, surname, userPatientLinkToken, DefaultOdsCode, DefaultAssociationType));

            _context.SetSessionDetails(firstName, surname);
        }

        [Given(@"I have incomplete OAuth details")]
        public void GivenIhaveahaveincompleteOAuthdetails()
        {
            // Incomplete will validate and return with an error
            // No setup required
        }

        [Given(@"I have invalid OAuth details")]
        public async Task GivenIhaveahaveinvalidOAuthdetails()
        {
            await PostMapping(TokenConfigurator
                .ForRequest(AuthCode, CodeVerifier)
                .RespondWithBadRequest());
        }

        [Given(@"I have valid OAuth details but EMIS is unavailable")]
        public async Task GivenIhavevalidOAuthdetailsbutEMISisunavailable()
        {
            await PostMapping(TokenConfigurator
                .ForRequest(AuthCode, CodeVerifier)
                .RespondWithSuccess(AccessToken));

            await PostMapping(UserProfileConfigurator
                .ForRequest(BearerToken)
                .RespondWithSuccess(DefaultOdsCode, ConnectionToken));

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithServiceUnavailable());
        }

        [Given(@"I have valid OAuth details and the CID tokens endpoint fails to process the request")]
        public async Task GivenIhavevalidOAuthdetailsandtheCIDtokensendpointfailstoprocesstherequest()
        {
            await PostMapping(TokenConfigurator
                .ForRequest(AuthCode, CodeVerifier)
                .RespondWithServerError());
        }

        [Given(@"I have valid OAuth details and the CID user profile endpoint fails to process the request")]
        public async Task GivenIhavevalidOAuthdetailsandtheCIDuserprofileendpointfailstoprocesstherequest()
        {
            await PostMapping(TokenConfigurator
                .ForRequest(AuthCode, CodeVerifier)
                .RespondWithSuccess(AccessToken));

            await PostMapping(UserProfileConfigurator
                .ForRequest(BearerToken)
                .RespondWithServerError());
        }

        [Given(@"I have valid OAuth details and the EMIS end user session endpoint fails to create")]
        public async Task GivenIhavevalidOAuthdetailsandtheEMISendusersessionendpointfailstocreate()
        {
            await PostMapping(TokenConfigurator
                .ForRequest(AuthCode, CodeVerifier)
                .RespondWithSuccess(AccessToken));

            await PostMapping(UserProfileConfigurator
                .ForRequest(BearerToken)
                .RespondWithSuccess(DefaultOdsCode, DefaultConnectionToken));

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithBadGatewayError());
        }

        [Given(@"I have valid OAuth details and the EMIS session endpoint fails to create")]
        public async Task GivenIhavevalidOAuthdetailsandtheEMISsessionendpointfailstocreate()
        {
            const string odsCode = DefaultOdsCode;
            const string endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D";

            await PostMapping(TokenConfigurator
                .ForRequest(AuthCode, CodeVerifier)
                .RespondWithSuccess(AccessToken));

            await PostMapping(UserProfileConfigurator
                .ForRequest(BearerToken)
                .RespondWithSuccess(DefaultOdsCode, ConnectionToken));

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(SessionConfigurator
                .ForRequest(ConnectionToken, odsCode, endUserSessionId)
                .RespondWithBadGateway());
        }

        [Given(@"I have valid OAuth details and the EMIS is unavailable")]
        public async Task GivenIhavevalidOAuthdetailsandtheEMISisunavailable()
        {
            await PostMapping(TokenConfigurator
                .ForRequest(AuthCode, CodeVerifier)
                .RespondWithSuccess(AccessToken));

            await PostMapping(UserProfileConfigurator
                .ForRequest(BearerToken)
                .RespondWithSuccess(DefaultOdsCode, DefaultConnectionToken));

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithServiceUnavailable());
        }

        [When(@"I create a user session with valid details")]
        public async Task WhenIcreateausersessionwithvaliddetails()
        {
            try
            {
                var result = await _context
                    .GetWorkerClient()
                    .PostSession(new UserSessionRequest
                    {
                        AuthCode = AuthCode,
                        CodeVerifier = CodeVerifier
                    });

                _context.Add(GetUserSessionResult, result);
            }
            catch (NhsoHttpException httpException)
            {
                Console.WriteLine(httpException);

                _context.SetHttpException(httpException);
            }
        }

        [When(@"I create a user session with incomplete details")]
        public async Task WhenIcreateausersessionwithincompletedetails()
        {
            try
            {
                var result = await _context
                    .GetWorkerClient()
                    .PostSession(new UserSessionRequest
                    {
                        AuthCode = null,
                        CodeVerifier = CodeVerifier
                    });

                _context.Add(GetUserSessionResult, result);
            }
            catch (NhsoHttpException httpException)
            {
                Console.WriteLine(httpException);

                _context.SetHttpException(httpException);
            }
        }

        [When("I create a user session with invalid details")]
        public async Task WhenIcreateausersessionwithinvaliddetails()
        {
            try
            {
                var result = await _context
                    .GetWorkerClient()
                    .PostSession(new UserSessionRequest
                    {
                        AuthCode = AuthCode,
                        CodeVerifier = CodeVerifier
                    });

                _context.Add(GetUserSessionResult, result);
            }
            catch (NhsoHttpException httpException)
            {
                Console.WriteLine(httpException);

                _context.SetHttpException(httpException);
            }
        }

        [Then(@"I receive a session id, given name and family name")]
        public void ThenIReceiveASessionIdGivenNameAndFamilyName()
        {
            const string cookieName = "NHSO-Session-Id";
            const string httpOnly = "httponly";
            var result = _context.Get<SessionResponseTestObject>(GetUserSessionResult);

            result.Should().NotBeNull();
            result.UserSessionResponse.GivenName.Should().Be(_context.GetSessionDetails().GivenName);
            result.UserSessionResponse.FamilyName.Should().Be(_context.GetSessionDetails().FamilyName);

            result.Cookie.Should().NotBeNull();
            result.Cookie.Should().Contain(cookieName);
            result.Cookie.Should().Contain(httpOnly);
        }

        [Given(@"I have invalid OAuth details and CID connection token fails to authenticate with emis")]
        public async Task GivenIhaveinvalidOAuthdetailsandCIDconnectiontokenfailstoauthenticatewithemis()
        {
            await PostMapping(TokenConfigurator
                .ForRequest(AuthCode, CodeVerifier)
                .RespondWithSuccess(AccessToken));

            await PostMapping(UserProfileConfigurator
                .ForRequest(BearerToken)
                .RespondWithSuccess(DefaultOdsCode, ConnectionToken));

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(SessionConfigurator
                .ForRequest(ConnectionToken, DefaultOdsCode, endUserSessionId)
                .RespondWithForbidden());

        }

        [Given(@"I have valid OAuth details and emis fails to respond in 30 seconds")]
        public async Task GivenIhavevalidOAuthdetailsandemisfailstoresponsein30seconds()
        {
            const string endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D";
            const int emisResponseDelay = 31;

            await PostMapping(TokenConfigurator
                .ForRequest(AuthCode, CodeVerifier)
                .RespondWithSuccess(AccessToken));

            await PostMapping(UserProfileConfigurator
                .ForRequest(BearerToken)
                .RespondWithSuccess(DefaultOdsCode, DefaultConnectionToken));

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithDelayedSuccess(endUserSessionId, emisResponseDelay));
        }

        private async Task PostMapping(Mapping mapping)
        {
            await _context.GetMockingClient()
                .PostMappingAsync(mapping);
        }
    }
}
