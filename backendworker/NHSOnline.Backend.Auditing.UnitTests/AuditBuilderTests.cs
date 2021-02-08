using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using UnitTestHelper;

namespace NHSOnline.Backend.Auditing.UnitTests
{
    [TestClass]
    public sealed class AuditBuilderTests
    {
        [TestMethod]
        public async Task Execute_CallsWriteAuditTwice()
        {
            var mockAuditSink = new Mock<IAuditSink>();

            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                await auditor
                    .Audit()
                    .NhsNumber("NhsNum")
                    .Supplier(Supplier.Unknown)
                    .Operation("Op")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(x => x.WritePreOperationAudit(It.IsAny<AuditRecord>()), Times.Exactly(1));
            mockAuditSink.Verify(x => x.WritePostOperationAudit(It.IsAny<AuditRecord>()), Times.Exactly(1));
        }

        [TestMethod]
        [DataRow("Subject")]
        [DataRow("NHS Login Id")]
        public async Task Execute_ValidAccessTokenString_AuditsNhsSubjectIdFromToken(string subject)
        {
            var mockAuditSink = new Mock<IAuditSink>();

            var accessToken = CreateAccessTokenString(subject);

            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                await auditor
                    .Audit()
                    .AccessToken(accessToken)
                    .NhsNumber("NhsNum")
                    .Supplier(Supplier.Unknown)
                    .Operation("Op")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.NhsLoginSubject == subject)),
                Times.Exactly(1));
            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.NhsLoginSubject == subject)),
                Times.Exactly(1));
        }

        [TestMethod]
        [DataRow("Subject")]
        [DataRow("NHS Login Id")]
        public async Task Execute_AccessTokenObject_AuditsNhsSubjectIdFromToken(string subject)
        {
            var mockAuditSink = new Mock<IAuditSink>();

            var accessToken = AccessToken.Parse(new Mock<ILogger>().Object, CreateAccessTokenString(subject));

            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                await auditor
                    .Audit()
                    .AccessToken(accessToken)
                    .NhsNumber("NhsNum")
                    .Supplier(Supplier.Unknown)
                    .Operation("Op")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.NhsLoginSubject == subject)),
                Times.Exactly(1));
            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.NhsLoginSubject == subject)),
                Times.Exactly(1));
        }

        [TestMethod]
        [DataRow("Subject")]
        [DataRow("NHS Login Id")]
        public async Task Execute_StartWithOperation_AuditsNhsLoginIdFromAccessTokenInScope(string subject)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var httpContext = ArrangeHttpContext(accessToken: CreateAccessTokenString(subject));

            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(httpContext))
            {
                await auditor
                    .Audit()
                    .Operation("operation")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.NhsLoginSubject == subject)),
                Times.Exactly(1));
            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.NhsLoginSubject == subject)),
                Times.Exactly(1));
        }

        [TestMethod]
        [DataRow("ABCD")]
        [DataRow("1234")]
        public async Task Execute_InvalidAccessTokenString_ThrowsNoAuditKeyException(string accessToken)
        {
            var mockAuditSink = new Mock<IAuditSink>();

            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                Func<Task<AuditedResultStub>> act = async () => await auditor
                    .Audit()
                    .AccessToken(accessToken)
                    .NhsNumber("NhsNum")
                    .Supplier(Supplier.Unknown)
                    .Operation("Op")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));

                await act.Should().ThrowAsync<NoAuditKeyException>();
            }
        }

        [TestMethod]
        [DataRow("123 456 7890")]
        [DataRow("NHS Number")]
        public async Task Execute_NhsNumber_AuditsNhsNumber(string nhsNumber)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                await auditor
                    .Audit()
                    .NhsNumber(nhsNumber)
                    .Supplier(Supplier.Unknown)
                    .Operation("Op")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.NhsNumber == nhsNumber)),
                Times.Exactly(1));
            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.NhsNumber == nhsNumber)),
                Times.Exactly(1));
        }

        [TestMethod]
        [DataRow("123 456 7890")]
        [DataRow("NHS Number")]
        public async Task Execute_StartWithOperation_AuditsNhsNumberFromUserSessionInScope(string nhsNumber)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var httpContext = ArrangeHttpContext(nhsNumber: nhsNumber);

            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(httpContext))
            {
                await auditor
                    .Audit()
                    .Operation("operation")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.NhsNumber == nhsNumber)),
                Times.Exactly(1));
            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.NhsNumber == nhsNumber)),
                Times.Exactly(1));
        }

        [TestMethod]
        [DataRow("123 456 7890", "098 765 4321")]
        [DataRow("NHS Number", "Proxy Nhs Number")]
        public async Task Execute_IsProxyDoesNotStartWithOperation_AuditsGivenNhsNumber(string nhsNumber, string proxyNhsNumber)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var httpContext = ArrangeHttpContext(isProxy: true, proxyNhsNumber: proxyNhsNumber);
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(httpContext))
            {
                await auditor
                    .Audit()
                    .NhsNumber(nhsNumber)
                    .Supplier(Supplier.Emis)
                    .Operation("Op")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.NhsNumber == nhsNumber)),
                Times.Exactly(1));
            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.NhsNumber == nhsNumber)),
                Times.Exactly(1));
        }

        [TestMethod]
        [DataRow("098 765 4321")]
        [DataRow("Proxy Nhs Number")]
        public async Task Execute_IsProxyStartWithOperation_AuditsNhsNumberFromProxyNhsNumberInHttpContext(string proxyNhsNumber)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var httpContext = ArrangeHttpContext(isProxy: true, proxyNhsNumber: proxyNhsNumber);
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(httpContext))
            {
                await auditor
                    .Audit()
                    .Operation("Op")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.NhsNumber == proxyNhsNumber)),
                Times.Exactly(1));
            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.NhsNumber == proxyNhsNumber)),
                Times.Exactly(1));
        }

        [TestMethod]
        [DataRow(Supplier.Unknown, "Unknown")]
        [DataRow(Supplier.Emis, "Emis")]
        public async Task Execute_Supplier_AuditsSupplier(Supplier supplier, string expectedSupplier)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                await auditor
                    .Audit()
                    .NhsNumber("NhsNumber")
                    .Supplier(supplier)
                    .Operation("Op")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.Supplier == expectedSupplier)),
                Times.Exactly(1));
            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.Supplier == expectedSupplier)),
                Times.Exactly(1));
        }

        [TestMethod]
        [DataRow(Supplier.Unknown, "Unknown")]
        [DataRow(Supplier.Emis, "Emis")]
        public async Task Execute_StartWithOperation_AuditsSupplierFromUserSessionInScope(Supplier supplier, string expectedSupplier)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var httpContext = ArrangeHttpContext(supplier: supplier);
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(httpContext))
            {
                await auditor
                    .Audit()
                    .Operation("Op")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.Supplier == expectedSupplier)),
                Times.Exactly(1));
            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.Supplier == expectedSupplier)),
                Times.Exactly(1));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Execute_DoesNotStartWithOperation_AuditsIsActingOnBehalfOfAnotherAsFalse(bool isProxy)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var httpContext = ArrangeHttpContext(isProxy: isProxy);
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(httpContext))
            {
                await auditor
                    .Audit()
                    .NhsNumber("nhsNumber")
                    .Supplier(Supplier.Emis)
                    .Operation("Op")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.IsActingOnBehalfOfAnother == false)),
                Times.Exactly(1));
            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.IsActingOnBehalfOfAnother == false)),
                Times.Exactly(1));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Execute_StartWithOperation_AuditsIsActingOnBehalfOfAnotherFromIsProxyInHttpContext(bool isProxy)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var httpContext = ArrangeHttpContext(isProxy: isProxy);
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(httpContext))
            {
                await auditor
                    .Audit()
                    .Operation("Op")
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.IsActingOnBehalfOfAnother == isProxy)),
                Times.Exactly(1));
            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.IsActingOnBehalfOfAnother == isProxy)),
                Times.Exactly(1));
        }

        [TestMethod]
        [DataRow("Operation", "Operation_Request")]
        [DataRow("Op_A", "Op_A_Request")]
        public async Task Execute_Operation_AuditsRequestOperation(string operation, string expectedOperation)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                await auditor
                    .Audit()
                    .NhsNumber("NhsNumber")
                    .Supplier(Supplier.Unknown)
                    .Operation(operation)
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.Operation == expectedOperation)),
                Times.Once);
        }

        [TestMethod]
        [DataRow("Operation", "Operation_Response")]
        [DataRow("Op_A", "Op_A_Response")]
        public async Task Execute_Operation_AuditsResponseOperation(string operation, string expectedOperation)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                await auditor
                    .Audit()
                    .NhsNumber("NhsNumber")
                    .Supplier(Supplier.Unknown)
                    .Operation(operation)
                    .Details("RequestDetails")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.Operation == expectedOperation)),
                Times.Once);
        }

        [TestMethod]
        [DataRow("Request Details")]
        [DataRow("Other Details")]
        public async Task Execute_Details_AuditsDetailsOnRequestOperation(string details)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                await auditor
                    .Audit()
                    .NhsNumber("NhsNumber")
                    .Supplier(Supplier.Unknown)
                    .Operation("Operation")
                    .Details(details)
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = "ResultDetails" }));
            }

            mockAuditSink.Verify(
                x => x.WritePreOperationAudit(It.Is<AuditRecord>(ar => ar.Operation == "Operation_Request" && ar.Details == details)),
                Times.Once);
        }

        [TestMethod]
        [DataRow("Response Details")]
        [DataRow("Other Details")]
        public async Task Execute_ResultDetails_AuditsDetailsOnResponseOperation(string details)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                await auditor
                    .Audit()
                    .NhsNumber("NhsNumber")
                    .Supplier(Supplier.Unknown)
                    .Operation("Operation")
                    .Details("Request Details")
                    .Execute(() => Task.FromResult(new AuditedResultStub { Details = details }));
            }

            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.Operation == "Operation_Response" && ar.Details == details)),
                Times.Once);
        }

        [TestMethod]
        [DataRow("Boom")]
        [DataRow("Exception")]
        public async Task Execute_ExceptionFromAction_PropagatesException(string exceptionMessage)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                var exception = new InvalidOperationException(exceptionMessage);

                Func<Task<AuditedResultStub>> act = async () => await auditor
                    .Audit()
                    .NhsNumber("NhsNumber")
                    .Supplier(Supplier.Unknown)
                    .Operation("Operation")
                    .Details("Request Details")
                    .Execute<AuditedResultStub>(() => throw exception);

                (await act.Should().ThrowAsync<InvalidOperationException>()).Which.Should().BeSameAs(exception);
            }
        }

        [TestMethod]
        [DataRow("Boom")]
        [DataRow("Exception")]
        public async Task Execute_ExceptionFromAction_AuditsExceptionOnResponseOperation(string exceptionMessage)
        {
            var mockAuditSink = new Mock<IAuditSink>();
            var auditor = CreateAuditor(mockAuditSink);

            using (auditor.BeginScope(new DefaultHttpContext()))
            {
                Func<Task<AuditedResultStub>> act = async () => await auditor
                    .Audit()
                    .NhsNumber("NhsNumber")
                    .Supplier(Supplier.Unknown)
                    .Operation("Operation")
                    .Details("Request Details")
                    .Execute<AuditedResultStub>(() => throw new InvalidOperationException(exceptionMessage));

                await act.Should().ThrowAsync<InvalidOperationException>();
            }

            mockAuditSink.Verify(
                x => x.WritePostOperationAudit(It.Is<AuditRecord>(ar => ar.Operation == "Operation_Response" && ar.Details == exceptionMessage)),
                Times.Once);
        }

        private static IAuditor CreateAuditor(Mock<IAuditSink> mockAuditSink)
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "AUDIT_SINK_TYPE", "FILE" } })
                .Build();
            new ServiceConfigurationModule().ConfigureServices(services, configuration);

            services.AddMockLoggers();
            services.AddSingleton<IConfiguration>(configuration);
            services.Replace(ServiceDescriptor.Singleton(mockAuditSink.Object));
            var serviceProvider = services.BuildServiceProvider();

            var auditor = serviceProvider.GetRequiredService<IAuditor>();
            return auditor;
        }

        private static string CreateAccessTokenString(string subject)
            => new JwtSecurityTokenHandler().CreateJwtSecurityToken(subject: new ClaimsIdentity(new[]
            {
                new Claim("nhs_number", "NHSNum"),
                new Claim("sub", subject)
            })).RawData;

        private HttpContext ArrangeHttpContext(
            string accessToken = null,
            string nhsNumber = "nhsNumber",
            Supplier supplier = Supplier.Unknown,
            bool? isProxy = null,
            string proxyNhsNumber = "proxyNhsNumber")
        {
            var gpUserSession = new Mock<GpUserSession>();
            gpUserSession.Object.NhsNumber = nhsNumber;
            gpUserSession.Setup(x => x.Supplier).Returns(supplier);

            var userSession = new P9UserSession(
                "csrfToken",
                nhsNumber,
                new CitizenIdUserSession
                {
                    AccessToken = accessToken ?? CreateAccessTokenString("Test"),
                },
                gpUserSession.Object,
                "im1ConnectionToken");

            var userSessionService = new Mock<IUserSessionService>();
            userSessionService.Setup(x => x.GetUserSession<UserSession>()).Returns(Option.Some<UserSession>(userSession));

            var services = new Mock<IServiceProvider>();
            services.Setup(x => x.GetService(typeof(IUserSessionService))).Returns(userSessionService.Object);

            var headers = new Mock<IHeaderDictionary>();

            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Headers).Returns(headers.Object);

            var httpContextItems = new Dictionary<object, object>();
            if (isProxy.HasValue)
            {
                httpContextItems["LinkedAccountAuditInfo"] = new LinkedAccountAuditInfo
                {
                    IsProxyMode = isProxy.Value,
                    ProxyNhsNumber = proxyNhsNumber
                };
            }

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.RequestServices).Returns(services.Object);
            httpContext.Setup(x => x.Request).Returns(request.Object);
            httpContext.Setup(x => x.Items).Returns(httpContextItems);
            return httpContext.Object;
        }

        private sealed class AuditedResultStub : IAuditedResult
        {
            public string Details { get; set;  }
        }
    }
}
