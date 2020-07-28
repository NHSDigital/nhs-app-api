using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing.UnitTestsSupport;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionControllerPostTests
    {
        private SessionControllerTestContext Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new SessionControllerTestContext();
        }

        [TestMethod]
        public async Task Post_CIDUserProfileCallFails_ReturnsBadRequest()
        {
            // Arrange
            Context.MockCitizenIdSessionService
                .Setup(x =>
                    x.Create(Context.UserSessionRequest.AuthCode, Context.UserSessionRequest.CodeVerifier, new Uri(Context.UserSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(new CitizenIdSessionResult
                {
                    StatusCode = (int) HttpStatusCode.BadRequest
                }))
                .Verifiable();

            Context.MockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.Is<ErrorTypes>(
                        et =>
                            et.Category == ErrorCategory.Login &&
                            et.StatusCode == StatusCodes.Status400BadRequest &&
                            et.SourceApi == SourceApi.None)))
                .Returns(SessionControllerTestContext.ServiceDeskReference)
                .Verifiable();

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = SessionControllerTestContext.ServiceDeskReference,
            };

            // Act
            var result = await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            // Assert
            Context.MockCitizenIdSessionService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            Context.MockAuditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Post_CIDUserProfileCallFails_WithBadGateway_ReturnsBadGateway()
        {
            // Arrange
            Context.MockCitizenIdSessionService
                .Setup(x =>
                    x.Create(Context.UserSessionRequest.AuthCode, Context.UserSessionRequest.CodeVerifier, new Uri(Context.UserSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(new CitizenIdSessionResult
                {
                    StatusCode = (int) HttpStatusCode.BadGateway
                }))
                .Verifiable();

            Context.MockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Login,
                    StatusCodes.Status502BadGateway, SourceApi.NhsLogin))
                .Returns(SessionControllerTestContext.ServiceDeskReference)
                .Verifiable();

            // Act
            var result = await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            // Assert
            Context.MockCitizenIdSessionService.Verify();
            result.Should().BeAssignableTo<ObjectResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            Context.MockAuditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Post_UnknownOdsCode_Returns464OdsCodeNotSupported()
        {
            // Arrange
            Context.MockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.IsAny<ErrorTypes.LoginOdsCodeNotFoundOrNotSupported>()))
                .Returns(SessionControllerTestContext.ServiceDeskReference)
                .Verifiable();

            ServiceJourneyRulesConfigResult serviceJourneyRulesConfigResult =
                new ServiceJourneyRulesConfigResult.Success(new ServiceJourneyRulesResponse
                {
                    Journeys = new Journeys { Supplier = Supplier.Unknown }
                });

            Context.MockServiceJourneyRulesService
                .Setup(x => x.GetServiceJourneyRulesForOdsCode(Context.UserProfile.OdsCode))
                .Returns(Task.FromResult(serviceJourneyRulesConfigResult))
                .Verifiable();

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = SessionControllerTestContext.ServiceDeskReference
            };

            var auditStub = ArrangeAudit();

            // Act
            var result = await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should()
                    .Be(Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
                auditStub.AccessTokenString.Should().Be(Context.CitizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(Context.UserProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Unknown);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Failed to determine the GP system based on ODS code 'OdsCode'");
            }

            Context.MockSessionControllerLogger.Verify();
        }

        [TestMethod]
        public async Task Post_GpSessionManagerReturnInvalidConnectionToken_ReturnsCreated()
        {
            // Arrange
            Context.MockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.GPSessionUnavailable>()))
                .Returns(SessionControllerTestContext.ServiceDeskReference)
                .Verifiable();

            var auditStub = ArrangeAudit();

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.InvalidConnectionToken());

            // Act
            var expectedValue = new PostUserSessionResponse()
            {
                Name = $"{Context.UserProfile.GivenName} {Context.UserProfile.FamilyName}",
                SessionTimeout = SessionControllerTestContext.SessionTimeoutSeconds,
                OdsCode = Context.UserProfile.OdsCode,
                DateOfBirth = Context.CitizenIdUserSession.DateOfBirth,
                NhsNumber = Context.UserProfile.NhsNumber,
                AccessToken = Context.UserProfile.AccessToken,
                ServiceJourneyRules = Context.ServiceJourneyRulesResponse,
                ProofLevel = ProofLevel.P9,
                Token = SessionControllerTestContext.CsrfRequestToken,
                UserSessionCreateReferenceCode = SessionControllerTestContext.ServiceDeskReference
            };
            var result = await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
                auditStub.AccessTokenString.Should().Be(Context.CitizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(Context.UserProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Session successfully created.");
            }

            Context.MockAuditor.Verify();
            Context.MockSessionControllerLogger.Verify();
        }

        [TestMethod]
        public async Task Post_GpSessionManagerReturnsForbidden_ReturnsCreated()
        {
            // Arrange
            Context.MockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.Is<ErrorTypes>(
                        et =>
                            et.Category == ErrorCategory.Login &&
                            et.StatusCode == Constants.CustomHttpStatusCodes.Status599GpSessionUnavailable &&
                            et.SourceApi == SourceApi.None)))
                .Returns(SessionControllerTestContext.ServiceDeskReference)
                .Verifiable();

            Context.UserSession.Key = "123";

            var auditStub = ArrangeAudit();

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Forbidden("Message"));

            // Act
            var expectedValue = new PostUserSessionResponse()
            {
                Name = $"{Context.UserProfile.GivenName} {Context.UserProfile.FamilyName}",
                SessionTimeout = SessionControllerTestContext.SessionTimeoutSeconds,
                OdsCode = Context.UserProfile.OdsCode,
                DateOfBirth = Context.CitizenIdUserSession.DateOfBirth,
                NhsNumber = Context.UserProfile.NhsNumber,
                AccessToken = Context.UserProfile.AccessToken,
                ServiceJourneyRules = Context.ServiceJourneyRulesResponse,
                ProofLevel = ProofLevel.P9,
                Token = SessionControllerTestContext.CsrfRequestToken,
                UserSessionCreateReferenceCode = SessionControllerTestContext.ServiceDeskReference
            };
            var result = await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
                auditStub.AccessTokenString.Should().Be(Context.CitizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(Context.UserProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Session successfully created.");
            }

            Context.MockAuditor.Verify();
            Context.MockSessionControllerLogger.Verify();
        }

        [DataTestMethod]
        [DynamicData(nameof(TestData), DynamicDataSourceType.Property)]
        public async Task Post_GpSupplierSessionCreateFails_ReturnsCreatedWithNoGpSession(
            GpSessionCreateResult gpSessionCreateResult,
            int expectedStatusCode)
        {
            // Arrange
            Context.MockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.Is<ErrorTypes>(
                        et =>
                            et.Category == ErrorCategory.Login &&
                            et.StatusCode == expectedStatusCode &&
                            et.SourceApi == SourceApi.Emis)))
                .Returns(SessionControllerTestContext.ServiceDeskReference)
                .Verifiable();

            var auditStub = ArrangeAudit();

            ArrangeGpSessionManagerCreateSession(gpSessionCreateResult);

            // Act
            var result = await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(expectedStatusCode);
                auditStub.AccessTokenString.Should().Be(Context.CitizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(Context.UserProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Session successfully created.");
            }

            Context.MockSessionControllerLogger.Verify();
        }

        private static IEnumerable<object[]> TestData
        {
            get
            {
                yield return new object [] { new GpSessionCreateResult.Timeout("Timeout"), 201 };
                yield return new object [] { new GpSessionCreateResult.Unparseable("Unparseable"), 201 };
                yield return new object [] { new GpSessionCreateResult.BadGateway("BadGateway"), 201};
                yield return new object[] { new GpSessionCreateResult.BadRequest("BadRequest"), 201 };
                yield return new object[] { new GpSessionCreateResult.InternalServerError("InternalServerError"), 201 };
            }
        }

        [TestMethod]
        public async Task Post_GetServiceJourneyRulesForOdsReturnsNotFound_Returns464InternalServerError()
        {
            // Arrange
            Context.SessionConfigSettings.ProxyEnabled = true;
            Context.EmisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };

            Context.MockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.LoginOdsCodeNotFoundOrNotSupported>()))
                .Returns(SessionControllerTestContext.ServiceDeskReference)
                .Verifiable();
            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = SessionControllerTestContext.ServiceDeskReference
            };
            ServiceJourneyRulesConfigResult serviceJourneyRulesConfigResult = new ServiceJourneyRulesConfigResult.NotFound();

            Context.MockServiceJourneyRulesService
                .Setup(x =>
                    x.GetServiceJourneyRulesForOdsCode(Context.UserProfile.OdsCode))
                .Returns(Task.FromResult(serviceJourneyRulesConfigResult))
                .Verifiable();

            Context.MockAuditor.Setup(x => x.Audit()).Returns(new AuditBuilderStub());

            // Act
            var result = await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            Context.MockServiceJourneyRulesService.Verify();
            Context.MockSessionControllerLogger.Verify();
        }

        [TestMethod]
        public async Task Post_GetServiceJourneyRulesForOdsReturnsInternalServerError_Returns500InternalServerError()
        {
            // Arrange
            Context.SessionConfigSettings.ProxyEnabled = true;
            Context.EmisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };

            Context.MockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.LoginServiceJourneyRulesOtherError>()))
                .Returns(SessionControllerTestContext.ServiceDeskReference)
                .Verifiable();

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = SessionControllerTestContext.ServiceDeskReference
            };

            ServiceJourneyRulesConfigResult serviceJourneyRulesConfigResult = new ServiceJourneyRulesConfigResult.InternalServerError();

            Context.MockServiceJourneyRulesService
                .Setup(x =>
                    x.GetServiceJourneyRulesForOdsCode(Context.UserProfile.OdsCode))
                .Returns(Task.FromResult(serviceJourneyRulesConfigResult))
                .Verifiable();

            Context.MockAuditor.Setup(x => x.Audit()).Returns(new AuditBuilderStub());

            // Act
            var result = await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            Context.MockServiceJourneyRulesService.Verify();
            Context.MockSessionControllerLogger.Verify();
        }

        [TestMethod]
        public async Task Post_HappyPath_ReturnsUsersSessionResponse()
        {
            Context.SessionConfigSettings.ProxyEnabled = true;

            Context.EmisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };

            var gpUserSession = new EmisUserSession
            {
                Im1MessagingEnabled = true,
                Name = $"{Context.UserProfile.GivenName} {Context.UserProfile.FamilyName}",
                NhsNumber = Context.UserProfile.NhsNumber
            };

            Context.UserSession.Key = "123";

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Success(gpUserSession));
            ArrangeAudit();

            // Act
            var result = await CreateSystemUnderTest().Post(Context.UserSessionRequest);


            // Assert
            var expectedUserSessionResponse = new PostUserSessionResponse
            {
                Name = $"{Context.UserProfile.GivenName} {Context.UserProfile.FamilyName}",
                SessionTimeout = SessionControllerTestContext.SessionTimeoutSeconds,
                OdsCode = Context.UserProfile.OdsCode,
                DateOfBirth = Context.CitizenIdUserSession.DateOfBirth,
                NhsNumber = Context.UserProfile.NhsNumber,
                AccessToken = Context.UserProfile.AccessToken,
                ServiceJourneyRules = Context.ServiceJourneyRulesResponse
            };

            using (new AssertionScope())
            {
                var createdResultValue = result.Should().BeAssignableTo<CreatedResult>().Subject.Value;
                var actualUserSessionResponse = createdResultValue.Should().BeAssignableTo<PostUserSessionResponse>().Subject;
                actualUserSessionResponse.Name.Should().Be(expectedUserSessionResponse.Name);
                actualUserSessionResponse.SessionTimeout.Should().Be(expectedUserSessionResponse.SessionTimeout);
                actualUserSessionResponse.Token.Should().Be(SessionControllerTestContext.CsrfRequestToken);
                actualUserSessionResponse.OdsCode.Should().Be(expectedUserSessionResponse.OdsCode);
                actualUserSessionResponse.DateOfBirth.Should().Be(expectedUserSessionResponse.DateOfBirth);
                actualUserSessionResponse.NhsNumber.Should().Be(expectedUserSessionResponse.NhsNumber);
                actualUserSessionResponse.AccessToken.Should().Be(expectedUserSessionResponse.AccessToken);
                actualUserSessionResponse.Im1MessagingEnabled.Should().BeTrue();
                actualUserSessionResponse.UserSessionCreateReferenceCode.Should().BeNull();
                actualUserSessionResponse.ServiceJourneyRules.Should().BeEquivalentTo(expectedUserSessionResponse.ServiceJourneyRules);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(TestDataServiceReference), DynamicDataSourceType.Property)]
        public async Task Post_HappyPathNoGpSession_ReturnsUsersSessionResponse_WithServiceDeskReference(
            GpSessionCreateResult gpSessionCreateResult,
            string expectedServiceDeskReference)
        {
            Context.UserSession.Key = "123";

            ArrangeGpSessionManagerCreateSession(gpSessionCreateResult);
            ArrangeAudit();

            Context.MockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.IsAny<ErrorTypes>()))
                .Returns(expectedServiceDeskReference)
                .Verifiable();

            // Act
            var result = await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            // Assert
            var expectedUserSessionResponse = new PostUserSessionResponse
            {
                Name = $"{Context.UserProfile.GivenName} {Context.UserProfile.FamilyName}",
                SessionTimeout = SessionControllerTestContext.SessionTimeoutSeconds,
                OdsCode = Context.UserProfile.OdsCode,
                DateOfBirth = Context.CitizenIdUserSession.DateOfBirth,
                NhsNumber = Context.UserProfile.NhsNumber,
                AccessToken = Context.UserProfile.AccessToken,
                ServiceJourneyRules = Context.ServiceJourneyRulesResponse
            };

            using (new AssertionScope())
            {
                var createdResultValue = result.Should().BeAssignableTo<CreatedResult>().Subject.Value;
                var actualUserSessionResponse = createdResultValue.Should().BeAssignableTo<PostUserSessionResponse>().Subject;
                actualUserSessionResponse.Name.Should().Be(expectedUserSessionResponse.Name);
                actualUserSessionResponse.SessionTimeout.Should().Be(expectedUserSessionResponse.SessionTimeout);
                actualUserSessionResponse.Token.Should().Be(SessionControllerTestContext.CsrfRequestToken);
                actualUserSessionResponse.OdsCode.Should().Be(expectedUserSessionResponse.OdsCode);
                actualUserSessionResponse.DateOfBirth.Should().Be(expectedUserSessionResponse.DateOfBirth);
                actualUserSessionResponse.NhsNumber.Should().Be(expectedUserSessionResponse.NhsNumber);
                actualUserSessionResponse.AccessToken.Should().Be(expectedUserSessionResponse.AccessToken);
                actualUserSessionResponse.Im1MessagingEnabled.Should().BeFalse();
                actualUserSessionResponse.UserSessionCreateReferenceCode.Should().Contain(expectedServiceDeskReference);
                actualUserSessionResponse.UserSessionCreateReferenceCode.Length.Equals(6);
                actualUserSessionResponse.ServiceJourneyRules.Should().BeEquivalentTo(expectedUserSessionResponse.ServiceJourneyRules);
            }
        }

        private static IEnumerable<object[]> TestDataServiceReference
        {
            get
            {
                yield return new object [] { new GpSessionCreateResult.Timeout("Timeout"), "ze" };
                yield return new object [] { new GpSessionCreateResult.Unparseable("Unparseable"), "3u" };
                yield return new object [] { new GpSessionCreateResult.BadGateway("BadGateway"), "3e"};
                yield return new object [] { new GpSessionCreateResult.BadGateway("BadGateway"), "3t"};
                yield return new object [] { new GpSessionCreateResult.BadGateway("BadGateway"), "3m"};
                yield return new object [] { new GpSessionCreateResult.BadGateway("BadGateway"), "3s"};
                yield return new object[]  { new GpSessionCreateResult.BadRequest("BadRequest"), "3a" };
                yield return new object[]  { new GpSessionCreateResult.InternalServerError("InternalServerError"), "3h" };
            }
        }

        [TestMethod]
        public async Task Post_HappyPath_CallsMetricLoggerLogin()
        {
            Context.SessionConfigSettings.ProxyEnabled = true;

            Context.EmisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };

            Context.UserSession.Key = "123";

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Success(Context.UserSession.GpUserSession));
            ArrangeAudit();

            // Act
            await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            // Assert
            Context.MockMetricLogger.Verify(x => x.Login(), Times.Once);
        }

        [TestMethod]
        public async Task Post_Im1ConnectionTokenHasCacheKey_AttemptsToDeleteFromCache()
        {
            // Arrange
            Context.SessionConfigSettings.ProxyEnabled = true;
            Context.EmisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };
            Context.UserSession.Key = "123";

            Context.MockIm1CacheService
                .Setup(x => x.DeleteIm1ConnectionToken(Context.ConnectionToken.Im1CacheKey))
                .Returns(Task.FromResult(true));

            ArrangeAudit();

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Success(Context.UserSession.GpUserSession));

            // Act
            await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            // Assert
            Context.MockIm1CacheService.Verify();
        }

        [TestMethod]
        public async Task Post_Im1ConnectionTokenIsAGuid_DoesNotAttemptToDeleteFromCache()
        {
            // Arrange
            Context.UserInfo.Im1ConnectionToken  = "0E5DD1C4-C519-4EF5-9B0F-624357F6F26F";

            Context.MockCitizenIdSessionService
                .Setup(x => x.Create(Context.UserSessionRequest.AuthCode, Context.UserSessionRequest.CodeVerifier, new Uri(Context.UserSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(Context.CitizenIdSessionResult));

            Context.MockIm1CacheService.Reset();
            Context.MockIm1CacheService
                .Setup(x => x.DeleteIm1ConnectionToken(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            ArrangeAudit();

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Success(Context.UserSession.GpUserSession));

            // Act
            await CreateSystemUnderTest().Post(Context.UserSessionRequest);

            // Assert
            Context.MockIm1CacheService.Verify(x => x.DeleteIm1ConnectionToken(It.IsAny<string>()), Times.Never);
        }

        private void ArrangeGpSessionManagerCreateSession(GpSessionCreateResult returnResult)
        {
            Context.MockGpSessionManager
                .Setup(
                    x => x.CreateSession(
                        It.Is<IGpSystem>(p => ReferenceEquals(p, Context.MockGpSystem.Object)),
                        It.Is<IGpSessionCreateArgs>(p
                            => p.NhsNumber.Equals(Context.CitizenIdSessionResult.NhsNumber, StringComparison.Ordinal) &&
                               p.OdsCode.Equals(Context.CitizenIdSessionResult.Session.OdsCode, StringComparison.Ordinal) &&
                               p.Im1ConnectionToken.Equals(Context.CitizenIdSessionResult.Im1ConnectionToken, StringComparison.Ordinal))))
                .ReturnsAsync(returnResult);
        }

        private AuditBuilderStub ArrangeAudit() => Context.ArrangeAudit();

        private SessionController CreateSystemUnderTest() => Context.CreateSystemUnderTest();
    }
}