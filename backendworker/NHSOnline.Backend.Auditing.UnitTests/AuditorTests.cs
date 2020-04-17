using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using UnitTestHelper;

namespace NHSOnline.Backend.Auditing.UnitTests
{
    [TestClass]
    public sealed class AuditorTests : IDisposable
    {
        private static string _nhsNumber1;
        private static string _nhsNumber2;
        private const Supplier SupplierEmis = Supplier.Emis;
        private static readonly string AccessToken = AuditorTestResources.AccessTokenValid;

        private IFixture _fixture;
        private Stream _stream;
        private DummyClassThatAudits _systemUnderTest;
        private ActionExecutingContext _actionExecutingContext;
        private ResultExecutedContext _resultContext;
        private IServiceProvider _requestServices;
        private Mock<IUserSessionService> _mockUserSessionService;

        private class UnknownSupplierSession : GpUserSession
        {
            public override Supplier Supplier => Supplier.Unknown;
            public override bool HasLinkedAccounts => false;
        }
        
        private class NestedCallClass
        {
            private readonly IAuditor _auditor;
            public bool PauseNestedExecution { get; set; }

            public NestedCallClass(IAuditor auditor)
            {
                _auditor = auditor;
            }

            public async Task TaskAsyncMethod()
            {
                await TaskedMethod();
            }

            private Task<int> TaskedMethod()
            {
                while (PauseNestedExecution)
                {
                    Thread.Sleep(10);
                }
                _auditor.Audit("Testing", "TaskedMethod");
                return Task.FromResult(1);
            }
        }

        private class DummyClassThatAudits
        {
            private readonly IAuditor _auditor;
            private readonly NestedCallClass _nestedClass;
            private readonly IServiceProvider _requestServices;
            private readonly Mock<IUserSessionService> _mockUserSessionService;

            public DummyClassThatAudits(
                IAuditor auditor,
                NestedCallClass nestedClass,
                IServiceProvider requestServices,
                Mock<IUserSessionService> mockUserSessionService)
            {
                _auditor = auditor;
                _nestedClass = nestedClass;
                _requestServices = requestServices;
                _mockUserSessionService = mockUserSessionService;
            }

            public async Task BasicAudit()
            {
                await _auditor.Audit("Testing", "woke up.");
            }

            public async Task ClearTextAudit()
            {
                const string param0 = "eggs";
                const int param1 = 5;
                await _auditor.Audit("Testing", "Had breakfast of {0} and {1} sausages.", param0, param1);
            }

            public async Task NestedControllerMethod()
            {
                _nestedClass.PauseNestedExecution = true;
                var awaiter = Task.Run(_nestedClass.TaskAsyncMethod);

                var mockUserSessionService = new Mock<IUserSessionService>();
                mockUserSessionService
                    .Setup(x => x.GetUserSession<UserSession>())
                    .Returns(Option.Some(CreateUserSession(_nhsNumber2, AuditorTestResources.AccessTokenValid)));
                var requestServices = new ServiceCollection()
                    .AddSingleton(mockUserSessionService.Object)
                    .BuildServiceProvider();

                var dummyContext = new DefaultHttpContext { RequestServices = requestServices };

                using (_auditor.BeginScope(dummyContext))
                {
                    await _auditor.Audit("Testing", "Message with rubbish scope 1");
                    _nestedClass.PauseNestedExecution = false;
                    await awaiter;

                    await _auditor.Audit("Testing", "Message with rubbish scope 2");
                }
            }

            public async Task Audit(
                UserSession userSessionInContext,
                string operation, 
                string details,
                params object[] parameters
            )
            {
                var dummyContext = new DefaultHttpContext { RequestServices = _requestServices };
                _mockUserSessionService
                    .Setup(x => x.GetUserSession<UserSession>())
                    .Returns(Option.Some(userSessionInContext));

                using (_auditor.BeginScope(dummyContext))
                {
                    await _auditor.Audit(operation, details, parameters);                    
                }
            }
            
            public async Task AuditRegistrationEvent(
                string nhsNumber, 
                Supplier supplier,
                string operation, 
                string details,
                params object[] parameters
            )
            {
                var dummyContext = new DefaultHttpContext { RequestServices = _requestServices };

                using (_auditor.BeginScope(dummyContext))
                {
                    await _auditor.AuditRegistrationEvent(nhsNumber, supplier, operation, details, parameters);                    
                }
            }
            
            public async Task AuditSessionEvent(
                string accessToken, 
                string nhsNumber, 
                Supplier supplier,
                string operation, 
                string details,
                params object[] parameters
                )
            {
                var dummyContext = new DefaultHttpContext { RequestServices = _requestServices };

                using (_auditor.BeginScope(dummyContext))
                {
                    await _auditor.AuditSessionEvent(accessToken, nhsNumber, supplier, operation, details, parameters);                    
                }
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            
            _nhsNumber1 = _fixture.CreateNhsNumberFormatted();
            _nhsNumber2 = _fixture.CreateNhsNumberFormatted();
            
            // Set the stream for audits
            _stream = new MemoryStream();
            var logger = _fixture.Freeze<Mock<ILogger<Auditor>>>();

            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>(Constants.EnvironmentalVariables.VersionTag, "UNIT TEST") });
            
            _fixture.Inject(new AuditorFactory(new StreamAuditSink(_stream), configBuilder.Build()).CreateAuditor(logger.Object));

            _mockUserSessionService = _fixture.Freeze<Mock<IUserSessionService>>();
            _mockUserSessionService
                .Setup(x => x.GetUserSession<UserSession>())
                .Returns(Option.Some(CreateUserSession(_nhsNumber1, AuditorTestResources.AccessTokenValid)));

            var mockServiceProvider = _fixture.Freeze<Mock<IServiceProvider>>();
            mockServiceProvider
                .Setup(x => x.GetService(typeof(IUserSessionService)))
                .Returns(_mockUserSessionService.Object);

            _requestServices = mockServiceProvider.Object;

            // set up http contexts for both controller and calling attribute overloads..
            var actionContext = new ActionContext(new DefaultHttpContext(), new Microsoft.AspNetCore.Routing.RouteData(), new ControllerActionDescriptor());
            actionContext.HttpContext.Items.Add("LinkedAccountAuditInfo", CreateLinkedAccountAuditInfo(false, ""));
            actionContext.HttpContext.RequestServices = _requestServices;
            _actionExecutingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), _systemUnderTest);
            _resultContext = new ResultExecutedContext(actionContext, new List<IFilterMetadata>(), new ObjectResult(1), _systemUnderTest);

            // Create system under test from IOC injection...
            _systemUnderTest = _fixture.Create<DummyClassThatAudits>();
        }

        private static UserSession CreateUserSession(string nhsNumber, string accessToken)
        {
            return new P9UserSession(
                string.Empty,
                new CitizenIdUserSession { AccessToken = accessToken },
                new EmisUserSession { NhsNumber = nhsNumber },
                string.Empty);
        }

        private static LinkedAccountAuditInfo CreateLinkedAccountAuditInfo(bool isProxying, string proxyNhsNumber)
        {
            return new LinkedAccountAuditInfo
            {
                IsProxyMode = isProxying,
                ProxyNhsNumber = proxyNhsNumber
            };
        }

        private void RunControllerMethod(Func<Task> controllerMethod)
        {
            var attribute = _fixture.Create<HttpContextAuditActionFilterAttribute>();
            attribute.OnActionExecuting(_actionExecutingContext);
            controllerMethod().Wait();
            attribute.OnActionExecuted(null);// Method should do nothing so can pass null in...
            attribute.OnResultExecuting(null);// Method should do nothing so can pass null in...
            attribute.OnResultExecuted(_resultContext);
        }

        [TestCleanup]
        public void TestTeardown()
        {
            _stream.Close();
            _stream.Dispose();
        }

        [TestMethod]
        public void TestCanAuditBasicStaticStringMessage()
        {
            RunControllerMethod(_systemUnderTest.BasicAudit);

            _stream.Position = 0;
            var testString = new StreamReader(_stream).ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain("woke up.");
            testString.Should().Contain(_nhsNumber1);
            testString.Should().Contain(SupplierEmis.ToString());
        }

        [TestMethod]
        public void TestCanAuditMessageWithClearTextParameters()
        {
            RunControllerMethod(_systemUnderTest.ClearTextAudit);

            _stream.Position = 0;
            var testString = new StreamReader(_stream).ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain("Had breakfast of eggs and 5 sausages.");
            testString.Should().Contain(_nhsNumber1);
            testString.Should().Contain(SupplierEmis.ToString());
        }

        [TestMethod, ExpectedException(typeof(NoAuditKeyException))]
        public async Task TestThrowsExceptionIfAuditScopeNotSetUp()
        {
            await _systemUnderTest.BasicAudit();
        }

        [TestMethod]
        public async Task Audit_HappyPath()
        {
            await _systemUnderTest.Audit(
                CreateUserSession(_nhsNumber1, AuditorTestResources.AccessTokenValid),
                "Test Audit", "SomeDetails '{0} {1}'", "with", "parameters");

            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            var testString = streamReader.ReadLine();
            testString.Should().EndWith(AuditorTestResources.AccessTokenSubject + " | " +  _nhsNumber1 + " | False | Emis | Test Audit | SomeDetails 'with parameters' |");
        }

        [DataTestMethod, ExpectedException(typeof(NoAuditKeyException))]
        [DataRow(null)]
        [DataRow("")]
        public async Task Audit_NhsNumberNullOrEmpty_Throws(string nhsNumber)
        {
            var userSession = CreateUserSession(nhsNumber, AuditorTestResources.AccessTokenValid);
            await _systemUnderTest.Audit(
                userSession,
                "Test Audit", 
                "SomeDetails '{0} {1}'", 
                "with", "parameters");
        }

        [DataTestMethod, ExpectedException(typeof(NoAuditKeyException))]
        [DataRow(null)]
        [DataRow("")]
        public async Task Audit_AccessTokenNullOrEmpty_Throws(string accessToken)
        {
            var userSession = CreateUserSession(_nhsNumber1, accessToken);
            
            await _systemUnderTest.Audit(
                userSession,
                "Test Audit", 
                "SomeDetails '{0} {1}'", 
                "with", "parameters");
        }
        
        [TestMethod, ExpectedException(typeof(NoAuditKeyException))]
        public async Task Audit_AccessTokenInvalid_Throws()
        {
            var userSession = CreateUserSession(_nhsNumber1, AuditorTestResources.AccessTokenInvalid);
            
            await _systemUnderTest.Audit(
                userSession,
                "Test Audit", 
                "SomeDetails '{0} {1}'", 
                "with", "parameters");
        }
        
        [TestMethod]
        public async Task AuditSessionEvent_HappyPath()
        {
            await _systemUnderTest.AuditSessionEvent(
                AccessToken, 
                _nhsNumber1, 
                Supplier.Tpp, "Test Audit", "SomeDetails '{0} {1}'", "with", "parameters");

            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            var testString = streamReader.ReadLine();
            testString.Should().EndWith(AuditorTestResources.AccessTokenSubject + " | " +  _nhsNumber1 + " | False | Tpp | Test Audit | SomeDetails 'with parameters' |");
        }

        [DataTestMethod, ExpectedException(typeof(NoAuditKeyException))]
        [DataRow(null)]
        [DataRow("")]
        public async Task AuditSessionEvent_NhsNumberNullOrEmpty_Throws(string nhsNumber)
        {
            await _systemUnderTest.AuditSessionEvent(
                AccessToken, 
                nhsNumber, 
                Supplier.Vision, 
                "Test Audit", 
                "SomeDetails '{0} {1}'", 
                "with", "parameters");
        }

        [DataTestMethod, ExpectedException(typeof(NoAuditKeyException))]
        [DataRow(null)]
        [DataRow("")]
        public async Task AuditSessionEvent_AccessTokenNullOrEmpty_Throws(string accessToken)
        {
            await _systemUnderTest.AuditSessionEvent(
                accessToken, 
                _nhsNumber1, 
                SupplierEmis, 
                "Test Audit", 
                "SomeDetails '{0} {1}'", 
                "with", "parameters");
        }
        
        [TestMethod, ExpectedException(typeof(NoAuditKeyException))]
        public async Task AuditSessionEvent_AccessTokenInvalid_Throws()
        {
            await _systemUnderTest.AuditSessionEvent(
                AuditorTestResources.AccessTokenInvalid, 
                _nhsNumber1, SupplierEmis, 
                "Test Audit", 
                "SomeDetails '{0} {1}'", 
                "with", "parameters");
        }
        
        [TestMethod]
        public async Task AuditRegistrationEvent_HappyPath()
        {
            await _systemUnderTest.AuditRegistrationEvent(
                _nhsNumber1, 
                Supplier.Tpp, "Test Audit", "SomeDetails '{0} {1}'", "with", "parameters");

            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            var testString = streamReader.ReadLine();
            testString.Should().EndWith("|  | " +  _nhsNumber1 + " | False | Tpp | Test Audit | SomeDetails 'with parameters' |");
        }
        
        [DataTestMethod, ExpectedException(typeof(NoAuditKeyException))]
        [DataRow(null)]
        [DataRow("")]
        public async Task AuditRegistrationEvent_NhsNumberNullOrEmpty_Throws(string nhsNumber)
        {
            await _systemUnderTest.AuditRegistrationEvent(
                nhsNumber, 
                Supplier.Vision, 
                "Test Audit", 
                "SomeDetails '{0} {1}'", 
                "with", "parameters");
        }

        [TestMethod]
        public void TestCrossThreadAudits()
        {
            RunControllerMethod(_systemUnderTest.NestedControllerMethod);

            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            var auditLine1 = streamReader.ReadLine();
            auditLine1.Should().NotBeEmpty();
            auditLine1.Should().EndWith(_nhsNumber2 + " | False | Emis | Testing | Message with rubbish scope 1 |");

            var auditLine2 = streamReader.ReadLine();
            auditLine2.Should().NotBeEmpty();
            auditLine2.Should().EndWith(_nhsNumber1 + " | False | Emis | Testing | TaskedMethod |");

            var auditLine3  = streamReader.ReadLine();
            auditLine3.Should().NotBeEmpty();
            auditLine3.Should().EndWith(_nhsNumber2 + " | False | Emis | Testing | Message with rubbish scope 2 |");
        }
        
        
        [TestMethod]
        public void TestCrossThreadAuditsWhileProxying()
        {
            const string proxyNhsNumber = "123 456 7890";
            // set up http contexts for both controller and calling attribute overloads..
            var actionContext = new ActionContext(new DefaultHttpContext(), new Microsoft.AspNetCore.Routing.RouteData(), new ControllerActionDescriptor());
            actionContext.HttpContext.Items.Add("LinkedAccountAuditInfo", CreateLinkedAccountAuditInfo(true, proxyNhsNumber));
            actionContext.HttpContext.RequestServices = _requestServices;
            _actionExecutingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), _systemUnderTest);
            _resultContext = new ResultExecutedContext(actionContext, new List<IFilterMetadata>(), new ObjectResult(1), _systemUnderTest);

            RunControllerMethod(_systemUnderTest.NestedControllerMethod);

            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            var auditLine1 = streamReader.ReadLine();
            auditLine1.Should().NotBeEmpty();
            auditLine1.Should().EndWith(_nhsNumber2 + " | False | Emis | Testing | Message with rubbish scope 1 |");

            var auditLine2 = streamReader.ReadLine();
            auditLine2.Should().NotBeEmpty();
            auditLine2.Should().EndWith(proxyNhsNumber + " | True | Emis | Testing | TaskedMethod |");

            var auditLine3  = streamReader.ReadLine();
            auditLine3.Should().NotBeEmpty();
            auditLine3.Should().EndWith(_nhsNumber2 + " | False | Emis | Testing | Message with rubbish scope 2 |");
        }


        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
