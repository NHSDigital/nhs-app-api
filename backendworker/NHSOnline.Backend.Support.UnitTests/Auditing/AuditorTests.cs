using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support.Auditing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.ApiSupport.Filters;
using UnitTestHelper;

namespace NHSOnline.Backend.Support.UnitTests.Auditing
{
    [TestClass]
    public sealed class AuditorTests : IDisposable
    {
        private const string NhsNumber = "424 933 2837";
        private const Supplier SupplierEmis = Supplier.Emis;
        private const string RubbishScope = "THIS IS A RUBBISH AUDIT";

        private IFixture _fixture;
        private Stream _stream;
        private DummyClassThatAudits _systemUnderTest;
        ActionExecutingContext _actionExecutingContext;
        ResultExecutedContext _resultContext;

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

            public DummyClassThatAudits(IAuditor auditor, NestedCallClass nestedClass)
            {
                _auditor = auditor;
                _nestedClass = nestedClass;
            }

            public async Task BasicAudit()
            {
                await _auditor.Audit("Testing", "woke up.");
            }

            public async Task ClearTextAudit()
            {
                var param0 = "eggs";
                var param1 = 5;
                await _auditor.Audit("Testing", "Had breakfast of {0} and {1} sausages.", param0, param1);
            }

            public async Task NestedControllerMethod()
            {
                _nestedClass.PauseNestedExecution = true;
                var awaiter = Task.Run(_nestedClass.TaskAsyncMethod);

                var dummyContext = new DefaultHttpContext();
                dummyContext.Items.Add("UserSession", CreateUserSession(RubbishScope) );
                using (_auditor.BeginScope(dummyContext))
                {

                    await _auditor.Audit("Testing", "Message with rubbish scope 1");
                    _nestedClass.PauseNestedExecution = false;
                    await awaiter;

                    await _auditor.Audit("Testing", "Message with rubbish scope 2");
                }
            }

            public async Task AuditWithScope(string nhsNumber, Supplier supplier)
            {
                var dummyContext = new DefaultHttpContext();
                using (_auditor.BeginScope(dummyContext))
                {
                    await _auditor.AuditWithExplicitNhsNumber(nhsNumber, supplier, "Test Audit", "SomeDetails '{0} {1}'", "with", "parameters");                    
                }
            }
        }

        [TestInitialize]
        public void TestInitialise()
        {
            // Setup injection of factory.
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            // Set the stream for audits
            _stream = new MemoryStream();
            var logger = _fixture.Freeze<Mock<ILogger<Auditor>>>();

            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>(Constants.EnvironmentalVariables.VersionTag, "UNIT TEST") });
            
            _fixture.Inject(new AuditorFactory(new StreamAuditSink(_stream), configBuilder.Build()).CreateAuditor(logger.Object));

            // Create system under test from IOC injection...
            _systemUnderTest = _fixture.Create<DummyClassThatAudits>();

            // set up http contexts for both controller and calling attribute overloads..
            var actionContext = new ActionContext(new DefaultHttpContext(), new Microsoft.AspNetCore.Routing.RouteData(), new ControllerActionDescriptor());
            actionContext.HttpContext.Items.Add("UserSession", CreateUserSession(NhsNumber));
            _actionExecutingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<String, object>(), _systemUnderTest);
            _resultContext = new ResultExecutedContext(actionContext, new List<IFilterMetadata>(), new ObjectResult(1), _systemUnderTest);
        }

        private static UserSession CreateUserSession(string nhsNumber)
        {
            return new UserSession
            {
                GpUserSession = new EmisUserSession
                {
                    NhsNumber = nhsNumber
                }
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
            testString.Should().Contain(NhsNumber);
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
            testString.Should().Contain(NhsNumber);
            testString.Should().Contain(SupplierEmis.ToString());
        }

        [TestMethod, ExpectedException(typeof(NoAuditKeyException))]
        public async Task TestThrowsExceptionIfAuditScopeNotSetUp()
        {
            await _systemUnderTest.BasicAudit();
        }

        [TestMethod]
        public async Task TestAuditWithScopeProvidedInAuditMethod()
        {
            await _systemUnderTest.AuditWithScope(NhsNumber, Supplier.Tpp);

            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            var testString = streamReader.ReadLine();
            testString.Should().EndWith(NhsNumber + " | Tpp | Test Audit | SomeDetails 'with parameters' |");
        }

        [TestMethod, ExpectedException(typeof(NoAuditKeyException))]
        public async Task TestThrowsExceptionIfNhsNumberIsNull()
        {
            await _systemUnderTest.AuditWithScope("", Supplier.Vision);
        }

        [TestMethod, ExpectedException(typeof(NoAuditKeyException))]
        public async Task TestThrowsExceptionIfSupplierIsDefault()
        {
            await _systemUnderTest.AuditWithScope("1684156", Supplier.Unknown);
        }


        [TestMethod]
        public void TestCrossThreadAudits()
        {
            RunControllerMethod(_systemUnderTest.NestedControllerMethod);

            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            var auditLine1 = streamReader.ReadLine();
            auditLine1.Should().NotBeEmpty();
            auditLine1.Should().EndWith(RubbishScope + " | Emis | Testing | Message with rubbish scope 1 |");

            var auditLine2 = streamReader.ReadLine();
            auditLine2.Should().NotBeEmpty();
            auditLine2.Should().EndWith(NhsNumber + " | Emis | Testing | TaskedMethod |");

            var auditLine3  = streamReader.ReadLine();
            auditLine3.Should().NotBeEmpty();
            auditLine3.Should().EndWith(RubbishScope + " | Emis | Testing | Message with rubbish scope 2 |");
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
