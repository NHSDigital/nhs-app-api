using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing.UnitTestsSupport;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Session
{
    [TestClass]
    public class SessionCreatorTests
    {
        private SessionCreatorTestContext Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new SessionCreatorTestContext();
        }

        [TestMethod]
        public async Task CreateSession_CIDUserProfileCallReturnsBadRequest_ReturnsLoginBadRequestError()
        {
            // Arrange
            Context.ArrangeAntiforgery();
            Context.Mocks.CitizenIdSessionService
                .Setup(x => x.Create(
                    Context.Data.UserSessionRequest.AuthCode,
                    Context.Data.UserSessionRequest.CodeVerifier,
                    new Uri(Context.Data.UserSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(new CitizenIdSessionResult
                {
                    StatusCode = (int) HttpStatusCode.BadRequest
                }))
                .Verifiable();

            Context.Mocks.ErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.Is<ErrorTypes>(
                        et =>
                            et.Category == ErrorCategory.Login &&
                            et.StatusCode == StatusCodes.Status400BadRequest &&
                            et.SourceApi == SourceApi.None)))
                .Returns(SessionCreatorTestContext.ServiceDeskReference)
                .Verifiable();

            // Act
            var result = await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            using (new AssertionScope())
            {
                var errorResult = result.Should().BeAssignableTo<CreateSessionResult.ErrorResult>().Subject;

                errorResult.ErrorTypes.Category.Should().Be(ErrorCategory.Login);
                errorResult.ErrorTypes.Prefix.Should().BeEquivalentTo("3a");
                errorResult.ErrorTypes.SourceApi.Should().Be(SourceApi.None);
                errorResult.ErrorTypes.StatusCode.Should().Be(400);
            }

            Context.Mocks.CitizenIdSessionService.Verify();
            Context.Mocks.Auditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task CreateSession_CIDUserProfileCallReturnsBadGateway_ReturnsLoginBadGatewayError()
        {
            // Arrange
            Context.ArrangeAntiforgery();

            Context.Mocks.CitizenIdSessionService
                .Setup(x => x.Create(
                    Context.Data.UserSessionRequest.AuthCode,
                    Context.Data.UserSessionRequest.CodeVerifier,
                    new Uri(Context.Data.UserSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(new CitizenIdSessionResult
                {
                    StatusCode = (int) HttpStatusCode.BadGateway
                }))
                .Verifiable();

            Context.Mocks.ErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Login,
                    StatusCodes.Status502BadGateway, SourceApi.NhsLogin))
                .Returns(SessionCreatorTestContext.ServiceDeskReference)
                .Verifiable();

            // Act
            var result = await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            using (new AssertionScope())
            {
                var errorResult = result.Should().BeAssignableTo<CreateSessionResult.ErrorResult>().Subject;

                errorResult.ErrorTypes.Category.Should().Be(ErrorCategory.Login);
                errorResult.ErrorTypes.Prefix.Should().BeEquivalentTo("3n");
                errorResult.ErrorTypes.SourceApi.Should().Be(SourceApi.NhsLogin);
                errorResult.ErrorTypes.StatusCode.Should().Be(502);
            }

            Context.Mocks.CitizenIdSessionService.Verify();
            Context.Mocks.Auditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task CreateSession_UnknownOdsCode_ReturnsLoginOdsCodeNotSupportedError()
        {
            // Arrange
            var auditStub = ArrangeAudit();

            ServiceJourneyRulesConfigResult serviceJourneyRulesConfigResult =
                new ServiceJourneyRulesConfigResult.Success(new ServiceJourneyRulesResponse
                {
                    Journeys = new Journeys { Supplier = Supplier.Unknown }
                });

            Context.ArrangeAntiforgery();
            Context.ArrangeCitizenIdService();
            Context.ArrangeOdsCodeMassager();

            Context.Mocks.ErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.LoginOdsCodeNotFoundOrNotSupported>()))
                .Returns(SessionCreatorTestContext.ServiceDeskReference)
                .Verifiable();

            Context.Mocks.ServiceJourneyRulesService
                .Setup(x => x.GetServiceJourneyRulesForOdsCode(Context.Data.UserProfile.OdsCode))
                .Returns(Task.FromResult(serviceJourneyRulesConfigResult))
                .Verifiable();

            // Act
            var result = await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            using (new AssertionScope())
            {
                var errorResult = result.Should().BeAssignableTo<CreateSessionResult.ErrorResult>().Subject;

                errorResult.ErrorTypes.Category.Should().Be(ErrorCategory.Login);
                errorResult.ErrorTypes.Prefix.Should().BeEquivalentTo("3f");
                errorResult.ErrorTypes.SourceApi.Should().Be(SourceApi.None);
                errorResult.ErrorTypes.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber);

                auditStub.AccessTokenString.Should().Be(Context.Data.CitizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(Context.Data.UserProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Unknown);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Failed to determine the GP system based on ODS code 'OdsCode'");
            }
        }

        [TestMethod]
        public async Task CreateSession_GpSessionManagerReturnsInvalidConnectionToken_ReturnsSuccessResultWithNullGpSession()
        {
            // Arrange
            var auditStub = ArrangeAudit();
            var expectedUserSession = new P9UserSession(
                SessionCreatorTestContext.CsrfRequestToken,
                Context.Data.UserInfo.NhsNumber,
                Context.Data.CitizenIdSessionResult.Session,
                new NullGpSession(Supplier.Disconnected, SessionCreatorTestContext.ServiceDeskReference),
                Context.Data.CitizenIdSessionResult.Im1ConnectionToken)
            {
                Key = SessionCreatorTestContext.ApiSessionId
            };

            Context.ArrangeAntiforgery();
            Context.ArrangeCitizenIdService();
            Context.ArrangeOdsCodeMassager();
            Context.ArrangeGpSystemFactory();
            Context.ArrangeSessionCacheService();
            Context.ArrangeServiceJourneyRulesService();

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.InvalidConnectionToken());

            Context.Mocks.ErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.GPSessionUnavailable>()))
                .Returns(SessionCreatorTestContext.ServiceDeskReference)
                .Verifiable();

            // Act
            var result = await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            using (new AssertionScope())
            {
                var successResult = result.Should().BeAssignableTo<CreateSessionResult.Success>().Subject;

                expectedUserSession.OrganDonationSessionId = ((P9UserSession) successResult.UserSession).OrganDonationSessionId;

                successResult.ServiceJourneyRules.Should().Be(Context.Data.ServiceJourneyRulesResponse);
                successResult.UserSession.Should().BeEquivalentTo(expectedUserSession);

                auditStub.AccessTokenString.Should().Be(Context.Data.CitizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(Context.Data.UserProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Session successfully created.");
            }

            Context.Mocks.Auditor.Verify();
        }

        [TestMethod]
        public async Task CreateSession_GpSessionManagerReturnsForbidden_ReturnsSuccessResultWithNullGpSession()
        {
            // Arrange
            var auditStub = ArrangeAudit();
            var expectedUserSession = new P9UserSession(
                SessionCreatorTestContext.CsrfRequestToken,
                Context.Data.UserInfo.NhsNumber,
                Context.Data.CitizenIdSessionResult.Session,
                new NullGpSession(Supplier.Disconnected, SessionCreatorTestContext.ServiceDeskReference),
                Context.Data.CitizenIdSessionResult.Im1ConnectionToken)
            {
                Key = SessionCreatorTestContext.ApiSessionId
            };

            Context.ArrangeAntiforgery();
            Context.ArrangeCitizenIdService();
            Context.ArrangeOdsCodeMassager();
            Context.ArrangeGpSystemFactory();
            Context.ArrangeSessionCacheService();
            Context.ArrangeServiceJourneyRulesService();

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Forbidden("Message"));

            Context.Mocks.ErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.Is<ErrorTypes>(
                        et =>
                            et.Category == ErrorCategory.Login &&
                            et.StatusCode == Constants.CustomHttpStatusCodes.Status599GpSessionUnavailable &&
                            et.SourceApi == SourceApi.None)))
                .Returns(SessionCreatorTestContext.ServiceDeskReference)
                .Verifiable();

            Context.Data.UserSession.Key = "123";

            // Act
            var result = await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            using (new AssertionScope())
            {
                var successResult = result.Should().BeAssignableTo<CreateSessionResult.Success>().Subject;

                expectedUserSession.OrganDonationSessionId = ((P9UserSession) successResult.UserSession).OrganDonationSessionId;

                successResult.ServiceJourneyRules.Should().Be(Context.Data.ServiceJourneyRulesResponse);
                successResult.UserSession.Should().BeEquivalentTo(expectedUserSession);

                auditStub.AccessTokenString.Should().Be(Context.Data.CitizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(Context.Data.UserProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Session successfully created.");
            }

            Context.Mocks.Auditor.Verify();
        }

        [DataTestMethod]
        [DynamicData(nameof(TestData))]
        public async Task CreateSession_GpSupplierSessionCreateFails_ReturnsSuccessResultWithNullGpSession(
            GpSessionCreateResult gpSessionCreateResult)
        {
            // Arrange
            var auditStub = ArrangeAudit();
            var expectedUserSession = new P9UserSession(
                SessionCreatorTestContext.CsrfRequestToken,
                Context.Data.UserInfo.NhsNumber,
                Context.Data.CitizenIdSessionResult.Session,
                new NullGpSession(Supplier.Disconnected, SessionCreatorTestContext.ServiceDeskReference),
                Context.Data.CitizenIdSessionResult.Im1ConnectionToken)
            {
                Key = SessionCreatorTestContext.ApiSessionId
            };

            Context.ArrangeAntiforgery();
            Context.ArrangeCitizenIdService();
            Context.ArrangeOdsCodeMassager();
            Context.ArrangeGpSystemFactory();
            Context.ArrangeSessionCacheService();
            Context.ArrangeServiceJourneyRulesService();

            ArrangeGpSessionManagerCreateSession(gpSessionCreateResult);

            Context.Mocks.ErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.Is<ErrorTypes>(
                        et =>
                            et.Category == ErrorCategory.Login &&
                            et.StatusCode == 201 &&
                            et.SourceApi == SourceApi.Emis)))
                .Returns(SessionCreatorTestContext.ServiceDeskReference)
                .Verifiable();

            // Act
            var result = await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            using (new AssertionScope())
            {
                var successResult = result.Should().BeAssignableTo<CreateSessionResult.Success>().Subject;

                expectedUserSession.OrganDonationSessionId = ((P9UserSession) successResult.UserSession).OrganDonationSessionId;

                successResult.ServiceJourneyRules.Should().Be(Context.Data.ServiceJourneyRulesResponse);
                successResult.UserSession.Should().BeEquivalentTo(expectedUserSession);

                auditStub.AccessTokenString.Should().Be(Context.Data.CitizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(Context.Data.UserProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Session successfully created.");
            }
        }

        private static IEnumerable<object[]> TestData
        {
            get
            {
                yield return new object[] { new GpSessionCreateResult.Timeout("Timeout") };
                yield return new object[] { new GpSessionCreateResult.Unparseable("Unparseable") };
                yield return new object[] { new GpSessionCreateResult.BadGateway("BadGateway") };
                yield return new object[] { new GpSessionCreateResult.BadRequest("BadRequest") };
                yield return new object[] { new GpSessionCreateResult.InternalServerError("InternalServerError") };
            }
        }

        [TestMethod]
        public async Task CreateSession_GetServiceJourneyRulesForOdsReturnsNotFound_ReturnsLogin464Error()
        {
            // Arrange
            ServiceJourneyRulesConfigResult serviceJourneyRulesConfigResult = new ServiceJourneyRulesConfigResult.NotFound();

            Context.ArrangeAntiforgery();
            Context.ArrangeCitizenIdService();
            Context.ArrangeOdsCodeMassager();

            Context.Data.SessionConfigSettings.ProxyEnabled = true;
            Context.Data.EmisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };

            Context.Mocks.ErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.LoginOdsCodeNotFoundOrNotSupported>()))
                .Returns(SessionCreatorTestContext.ServiceDeskReference)
                .Verifiable();

            Context.Mocks.ServiceJourneyRulesService
                .Setup(x =>
                    x.GetServiceJourneyRulesForOdsCode(Context.Data.UserProfile.OdsCode))
                .Returns(Task.FromResult(serviceJourneyRulesConfigResult))
                .Verifiable();

            Context.Mocks.Auditor.Setup(x => x.Audit()).Returns(new AuditBuilderStub());

            // Act
            var result = await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            using (new AssertionScope())
            {
                var errorResult = result.Should().BeAssignableTo<CreateSessionResult.ErrorResult>().Subject;

                errorResult.ErrorTypes.Category.Should().Be(ErrorCategory.Login);
                errorResult.ErrorTypes.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber);
                errorResult.ErrorTypes.Prefix.Should().BeEquivalentTo("3f");
                errorResult.ErrorTypes.SourceApi.Should().Be(SourceApi.None);
            }

            Context.Mocks.ServiceJourneyRulesService.Verify();
        }

        [TestMethod]
        public async Task CreateSession_GetServiceJourneyRulesForOdsReturnsInternalServerError_ReturnsLogin500Error()
        {
            // Arrange
            ServiceJourneyRulesConfigResult serviceJourneyRulesConfigResult = new ServiceJourneyRulesConfigResult.InternalServerError();

            Context.ArrangeAntiforgery();
            Context.ArrangeCitizenIdService();
            Context.ArrangeOdsCodeMassager();

            Context.Data.SessionConfigSettings.ProxyEnabled = true;
            Context.Data.EmisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };

            Context.Mocks.ErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.LoginServiceJourneyRulesOtherError>()))
                .Returns(SessionCreatorTestContext.ServiceDeskReference)
                .Verifiable();

            Context.Mocks.ServiceJourneyRulesService
                .Setup(x =>
                    x.GetServiceJourneyRulesForOdsCode(Context.Data.UserProfile.OdsCode))
                .Returns(Task.FromResult(serviceJourneyRulesConfigResult))
                .Verifiable();

            Context.Mocks.Auditor.Setup(x => x.Audit()).Returns(new AuditBuilderStub());

            // Act
            var result = await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            using (new AssertionScope())
            {
                var errorResult = result.Should().BeAssignableTo<CreateSessionResult.ErrorResult>().Subject;

                errorResult.ErrorTypes.Category.Should().Be(ErrorCategory.Login);
                errorResult.ErrorTypes.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
                errorResult.ErrorTypes.Prefix.Should().BeEquivalentTo("3k");
                errorResult.ErrorTypes.SourceApi.Should().Be(SourceApi.ServiceJourneyRules);
            }

            Context.Mocks.ServiceJourneyRulesService.Verify();
        }

        [TestMethod]
        public async Task CreateSession_HappyPath_ReturnsSuccessResult()
        {
            // Arrange
            var auditStub = ArrangeAudit();

            Context.ArrangeAntiforgery();
            Context.ArrangeCitizenIdService();
            Context.ArrangeOdsCodeMassager();
            Context.ArrangeGpSystemFactory();
            Context.ArrangeSessionCacheService();
            Context.ArrangeServiceJourneyRulesService();

            Context.Data.SessionConfigSettings.ProxyEnabled = true;

            Context.Data.EmisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Success(Context.Data.EmisUserSession));

            // Act
            var result = await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            using (new AssertionScope())
            {
                var successResult = result.Should().BeAssignableTo<CreateSessionResult.Success>().Subject;

                var expectedUserSession = new P9UserSession(
                    SessionCreatorTestContext.CsrfRequestToken,
                    Context.Data.UserInfo.NhsNumber,
                    Context.Data.CitizenIdSessionResult.Session,
                    Context.Data.UserSession.GpUserSession,
                    Context.Data.CitizenIdSessionResult.Im1ConnectionToken)
                {
                    Key = SessionCreatorTestContext.ApiSessionId,
                    OrganDonationSessionId = ((P9UserSession) successResult.UserSession).OrganDonationSessionId
                };

                successResult.ServiceJourneyRules.Should().Be(Context.Data.ServiceJourneyRulesResponse);
                successResult.UserSession.Should().BeEquivalentTo(expectedUserSession);

                auditStub.AccessTokenString.Should().Be(Context.Data.CitizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(Context.Data.UserProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Session successfully created.");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(TestDataServiceReference))]
        public async Task CreateSession_HappyPathNoGpSession_ReturnsUserSessionWithNullGpSession(
            GpSessionCreateResult gpSessionCreateResult,
            string expectedServiceDeskReference)
        {
            // Arrange
            var auditStub = ArrangeAudit();

            Context.ArrangeGpSystemFactory();
            Context.ArrangeAntiforgery();
            Context.ArrangeCitizenIdService();
            Context.ArrangeOdsCodeMassager();
            Context.ArrangeSessionCacheService();
            Context.ArrangeServiceJourneyRulesService();

            ArrangeGpSessionManagerCreateSession(gpSessionCreateResult);

            Context.Mocks.ErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes>()))
                .Returns(expectedServiceDeskReference)
                .Verifiable();

            // Act
            var result = await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            using (new AssertionScope())
            {
                var successResult = result.Should().BeAssignableTo<CreateSessionResult.Success>().Subject;

                var expectedUserSession = new P9UserSession(
                    SessionCreatorTestContext.CsrfRequestToken,
                    Context.Data.UserInfo.NhsNumber,
                    Context.Data.CitizenIdSessionResult.Session,
                    new NullGpSession(Supplier.Disconnected, expectedServiceDeskReference),
                    Context.Data.CitizenIdSessionResult.Im1ConnectionToken)
                {
                    Key = SessionCreatorTestContext.ApiSessionId,
                    OrganDonationSessionId = ((P9UserSession) successResult.UserSession).OrganDonationSessionId
                };

                successResult.ServiceJourneyRules.Should().Be(Context.Data.ServiceJourneyRulesResponse);
                successResult.UserSession.Should().BeEquivalentTo(expectedUserSession);

                auditStub.AccessTokenString.Should().Be(Context.Data.CitizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(Context.Data.UserProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Session successfully created.");
            }
        }

        private static IEnumerable<object[]> TestDataServiceReference
        {
            get
            {
                yield return new object[] { new GpSessionCreateResult.Timeout("Timeout"), "ze" };
                yield return new object[] { new GpSessionCreateResult.Unparseable("Unparseable"), "3u" };
                yield return new object[] { new GpSessionCreateResult.BadGateway("BadGateway"), "3e" };
                yield return new object[] { new GpSessionCreateResult.BadGateway("BadGateway"), "3t" };
                yield return new object[] { new GpSessionCreateResult.BadGateway("BadGateway"), "3m" };
                yield return new object[] { new GpSessionCreateResult.BadGateway("BadGateway"), "3s" };
                yield return new object[] { new GpSessionCreateResult.BadRequest("BadRequest"), "3a" };
                yield return new object[] { new GpSessionCreateResult.InternalServerError("InternalServerError"), "3h" };
            }
        }

        [TestMethod]
        public async Task CreateSession_Im1ConnectionTokenHasCacheKey_AttemptsToDeleteFromCache()
        {
            // Arrange
            Context.ArrangeAntiforgery();
            Context.ArrangeCitizenIdService();
            Context.ArrangeOdsCodeMassager();
            Context.ArrangeGpSystemFactory();
            Context.ArrangeSessionCacheService();
            Context.ArrangeServiceJourneyRulesService();

            ArrangeAudit();

            Context.Data.SessionConfigSettings.ProxyEnabled = true;
            Context.Data.EmisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };

            Context.Mocks.Im1CacheService
                .Setup(x => x.DeleteIm1ConnectionToken(Context.Data.ConnectionToken.Im1CacheKey))
                .Returns(Task.FromResult(true));

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Success(Context.Data.UserSession.GpUserSession));

            // Act
            await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            Context.Mocks.Im1CacheService.Verify();
        }

        [TestMethod]
        public async Task CreateSession_Im1ConnectionTokenIsAGuid_DoesNotAttemptToDeleteFromCache()
        {
            // Arrange
            Context.ArrangeAntiforgery();
            Context.ArrangeSessionCacheService();
            Context.ArrangeOdsCodeMassager();
            Context.ArrangeGpSystemFactory();
            Context.ArrangeServiceJourneyRulesService();

            ArrangeAudit();

            Context.Data.UserInfo.Im1ConnectionToken  = "0E5DD1C4-C519-4EF5-9B0F-624357F6F26F";

            Context.Mocks.CitizenIdSessionService
                .Setup(x => x.Create(Context.Data.UserSessionRequest.AuthCode, Context.Data.UserSessionRequest.CodeVerifier, new Uri(Context.Data.UserSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(Context.Data.CitizenIdSessionResult));

            Context.Mocks.Im1CacheService.Reset();
            Context.Mocks.Im1CacheService
                .Setup(x => x.DeleteIm1ConnectionToken(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Success(Context.Data.UserSession.GpUserSession));

            // Act
            await CreateSystemUnderTest().CreateSession(Context.Data.CreateSessionRequest);

            // Assert
            Context.Mocks.Im1CacheService.Verify(x => x.DeleteIm1ConnectionToken(It.IsAny<string>()), Times.Never);
        }

        private void ArrangeGpSessionManagerCreateSession(GpSessionCreateResult returnResult)
        {
            Context.Mocks.GpSessionManager
                .Setup(
                    x => x.CreateSession(
                        It.Is<IGpSystem>(p => ReferenceEquals(p, Context.Mocks.GpSystem.Object)),
                        It.Is<IGpSessionCreateArgs>(p
                            => p.NhsNumber.Equals(Context.Data.CitizenIdSessionResult.NhsNumber, StringComparison.Ordinal) &&
                               p.OdsCode.Equals(Context.Data.CitizenIdSessionResult.Session.OdsCode, StringComparison.Ordinal) &&
                               p.Im1ConnectionToken.Equals(Context.Data.CitizenIdSessionResult.Im1ConnectionToken, StringComparison.Ordinal))))
                .ReturnsAsync(returnResult);
        }

        private AuditBuilderStub ArrangeAudit() => Context.ArrangeAudit();

        private SessionCreator CreateSystemUnderTest() => Context.CreateSystemUnderTest();
    }
}