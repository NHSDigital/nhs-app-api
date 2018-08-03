using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.UnitTests.Areas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;

namespace NHSOnline.Backend.Worker.UnitTests.Support.Auditing
{
    [TestClass]
    public sealed class AuditorTests : IDisposable
    {
        private const string NhsNumber = "123-2321-21312";
        private const Supplier Supplier = Worker.Supplier.Emis;
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

            public void BasicAudit()
            {
                _auditor.Audit("Testing", "woke up.");
            }

            public void ClearTextAudit()
            {
                var param0 = "eggs";
                var param1 = 5;
                _auditor.Audit("Testing", "Had breakfast of {0} and {1} sausages.", param0, param1);
            }

            public void NestedControllerMethod()
            {
                _nestedClass.PauseNestedExecution = true;
                var awaiter = Task.Run(_nestedClass.TaskAsyncMethod);

                var dummyContext = new DefaultHttpContext();
                dummyContext.Items.Add("UserSession", new EmisUserSession() { NhsNumber = RubbishScope });
                using (_auditor.BeginScope(dummyContext))
                {

                    _auditor.Audit("Testing", "Message with rubbish scope 1");
                    _nestedClass.PauseNestedExecution = false;
                    awaiter.Wait();

                    _auditor.Audit("Testing", "Message with rubbish scope 2");
                }
            }

            public void AuditWithScope(string nhsNumber, Supplier supplier)
            {
                _auditor.AuditWithExplicitNhsNumber(nhsNumber, supplier, "Test Audit", "SomeDetails '{0} {1}'", "with", "parameters");
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
            var logger = _fixture.Freeze<Mock<ILogger>>(); 
            _fixture.Inject(new AuditorFactory(new StreamAuditSink(_stream)).CreateAuditor(logger.Object));

            // Create system under test from IOC injection...
            _systemUnderTest = _fixture.Create<DummyClassThatAudits>();

            // set up http contexts for both controller and calling attribute overloads..
            var actionContext = new ActionContext(new DefaultHttpContext(), new Microsoft.AspNetCore.Routing.RouteData(), new ControllerActionDescriptor());
            actionContext.HttpContext.Items.Add("UserSession", new EmisUserSession() { NhsNumber = NhsNumber });
            _actionExecutingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<String, object>(), _systemUnderTest);
            _resultContext = new ResultExecutedContext(actionContext, new List<IFilterMetadata>(), new ObjectResult(1), _systemUnderTest);
        }

        private void RunControllerMethod(Action controllerMethod)
        {
            var attribute = _fixture.Create<HttpContextAuditActionFilterAttribute>();
            attribute.OnActionExecuting(_actionExecutingContext);
            controllerMethod();
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
        public void TestCanAuditBaiscStaticStringMessage()
        {
            RunControllerMethod(_systemUnderTest.BasicAudit);

            _stream.Position = 0;
            var testString = new StreamReader(_stream).ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain("woke up.");
            testString.Should().NotContain(NhsNumber);
            testString.Should().Contain(Supplier.ToString());
        }

        [TestMethod]
        public void TestCanAuditMessageWithClearTextParameters()
        {
            RunControllerMethod(_systemUnderTest.ClearTextAudit);

            _stream.Position = 0;
            var testString = new StreamReader(_stream).ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain("Had breakfast of eggs and 5 sausages.");
            testString.Should().NotContain(NhsNumber);
            testString.Should().Contain(Supplier.ToString());
        }

        [TestMethod, ExpectedException(typeof(NoAuditKeyException))]
        public void TestThrowsExceptionIfAuditScopeNotSetUp()
        {
            _systemUnderTest.BasicAudit();
        }

        [TestMethod]
        public void TestAuditWithScopeProvidedInAuditMethod()
        {
            _systemUnderTest.AuditWithScope("NHS_AppliedNumber", Supplier.Tpp);

            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            var testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            var splitLog = testString.Split(new char[] { '[', ']' });
            splitLog[1].Should().Be(AuditCryptographer.Hash("NHS_AppliedNumber").Trim(new char[] { '[', ']' }));
            splitLog[2].Should().Be(" | Tpp | Test Audit | SomeDetails 'with parameters' |");
        }


        [TestMethod, ExpectedException(typeof(NoAuditKeyException))]
        public void TestThrowsExceptionIfNhsNumberIsNull()
        {
            _systemUnderTest.AuditWithScope("", Supplier.Vision);
        }

        [TestMethod, ExpectedException(typeof(NoAuditKeyException))]
        public void TestThrowsExceptionIfSupplierIsDefault()
        {
            _systemUnderTest.AuditWithScope("1684156", Supplier.Unknown);
        }


        [TestMethod]
        public void TestCrossThreadAudits()
        {
            RunControllerMethod(_systemUnderTest.NestedControllerMethod);

            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            var testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            var splitLog = testString.Split(new char[] { '[', ']' });
            splitLog[1].Should().Be(AuditCryptographer.Hash(RubbishScope).Trim(new char[] { '[', ']' }));
            splitLog[2].Should().Be(" | Emis | Testing | Message with rubbish scope 1 |");

            testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            splitLog = testString.Split(new char[] { '[', ']' });
            splitLog[1].Should().Be(AuditCryptographer.Hash(NhsNumber).Trim(new char[] { '[', ']' }));
            splitLog[2].Should().Be(" | Emis | Testing | TaskedMethod |");

            testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            splitLog = testString.Split(new char[] { '[', ']' });
            splitLog[1].Should().Be(AuditCryptographer.Hash(RubbishScope).Trim(new char[] { '[', ']' }));
            splitLog[2].Should().Be(" | Emis | Testing | Message with rubbish scope 2 |");
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
