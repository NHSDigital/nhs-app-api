using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NHSOnline.Backend.Support.Logging;
using System.Threading;
using NHSOnline.Backend.ApiSupport.Filters;
using UnitTestHelper;

namespace NHSOnline.Backend.Support.UnitTests.Logging
{
    [TestClass]
    public sealed class HttpContexedLoggerTests : IDisposable
    {
        const string SessionId = "sfd-bnfg-sdf-dsgd-gf";
        const string MethodLogPrefix = "Test log ";
        private const string RegExt1 = "personalData=[^&]*";
        private const string RegExt2 = "gobbledygook=[^&]*";
        private const string RegExt3 = "hidden=[^&]*";

        private class NestedCallClass
        {
            private readonly ILogger _logger;
            public bool PauseNestedExecution { get; set; }

            public NestedCallClass(ILoggerFactory logProvider)
            {
                _logger = logProvider.CreateLogger<NestedCallClass>();
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

                _logger.LogCritical(MethodLogPrefix + "TaskedMethod");
                return Task.FromResult(1);
            }
        }


        private class DummyController : Controller
        {
            private readonly ILogger _logger;
            private readonly NestedCallClass _nestedClass;

            public DummyController(ILoggerFactory logProvider, NestedCallClass nestedClass)
            {
                _logger = logProvider.CreateLogger<DummyController>();
                _nestedClass = nestedClass;
            }

            public void ControllerMethod()
            {
                _logger.LogCritical(MethodLogPrefix + "ControllerMethod");
            }

            public void NestedControllerMethod()
            {
                _nestedClass.PauseNestedExecution = true;
                var awaiter = Task.Run(_nestedClass.TaskAsyncMethod);
                using (_logger.BeginScope("Rubbish Scope"))
                {
                    _nestedClass.PauseNestedExecution = false;
                    awaiter.Wait();

                    _logger.LogCritical("Message with rubbish scope");
                }
            }

            public void NonControllerMethod()
            {
                using (_logger.BeginScope(HttpContext))
                {
                    _logger.LogCritical(MethodLogPrefix + "NonControllerMethod");
                }
            }

            public async Task NestedCallMethod()
            {
                using (_logger.BeginScope("Rubbish"))
                {
                    using (_logger.BeginScope(HttpContext))
                    {
                        _nestedClass.PauseNestedExecution = false;
                        await _nestedClass.TaskAsyncMethod();
                        _logger.LogCritical(MethodLogPrefix + "NestedCallMethod");
                    }
                }
            }

            public void ControllerCatchExceptionMethod()
            {
                try
                {
                    throw new ArgumentException("Something has gone wrong!");
                }
                catch (ArgumentException exception)
                {
                    _logger.LogCritical(exception, "Logging an exception.");
                }
            }
            public void ControllerLogSensorFilterMethod()
            {
                _logger.LogCritical("Filter?personalData=sensitive&otherStuff=ok");
                using (_logger.BeginScope("Filter?otherStuff=ok&personalData=prohibited"))
                {
                    _logger.LogCritical("Filter?hidden=Asterix&otherStuff=ok");
                }
            }
        }

        private IFixture _fixture;
        private DummyController _systemUnderTest;
        private Stream _stream;

        private ActionExecutingContext _actionExecutingContext;
        private ResultExecutedContext _resultContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            // Setup for getting the log system back in injection for test
            var serviceProvider = new ServiceCollection()
                .AddLogging(ConfigureLogging)
                .BuildServiceProvider();
            _fixture.Inject(serviceProvider.GetService<ILoggerFactory>());

            // Create system under test from IOC injection...
            _systemUnderTest = _fixture.Create<DummyController>();

            // set up http contexts for both controller and calling attribute overloads..
            var actionContext = new ActionContext(new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(), new ControllerActionDescriptor());
            actionContext.HttpContext.Items.Add("UserSession", new UserSession() { Key = SessionId });
            _systemUnderTest.ControllerContext = new ControllerContext(actionContext);
            _actionExecutingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(),
                new Dictionary<string, object>(), _systemUnderTest);
            _resultContext = new ResultExecutedContext(actionContext, new List<IFilterMetadata>(), new ObjectResult(1),
                _systemUnderTest);
        }

        private void ConfigureLogging(ILoggingBuilder logBuilder)
        {
            var filters = new List<LogCensorFilter>
            {
                new LogCensorFilter { Match = RegExt1, Replacement = RegExt1.Replace("[^&]*", "*****", StringComparison.Ordinal) },
                new LogCensorFilter { Match = RegExt2, Replacement = RegExt2.Replace("[^&]*", "*****", StringComparison.Ordinal) },
                new LogCensorFilter { Match = RegExt3, Replacement = RegExt3.Replace("[^&]*", "*****", StringComparison.Ordinal) },
            };

            _stream = new MemoryStream();
            logBuilder.AddProvider(new HttpContexedLoggerProvider(new StreamWriter(_stream), LogLevel.Critical, LogLevel.None, filters));
        }

        [TestCleanup]
        public void TestTeardown()
        {
            _stream.Close();
            _stream.Dispose();
        }

        private string RunControllerMethod(Action controllerMethod)
        {
            var attribute = _fixture.Create<HttpContextLogActionFilterAttribute>();
            attribute.OnActionExecuting(_actionExecutingContext);
            controllerMethod();
            attribute.OnActionExecuted(null); // Method should do nothing so can pass null in...
            attribute.OnResultExecuting(null); // Method should do nothing so can pass null in...
            attribute.OnResultExecuted(_resultContext);
            return controllerMethod.Method.Name;
        }

        [TestMethod]
        public void LogFromControllerMethod()
        {
            var methodName = RunControllerMethod(_systemUnderTest.ControllerMethod);

            _stream.Position = 0;
            var testString = new StreamReader(_stream).ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain(SessionId);
            testString.Should().Contain(MethodLogPrefix + methodName);
        }

        [TestMethod]
        public void LogNestedControllerMethod()
        {
            RunControllerMethod(_systemUnderTest.NestedControllerMethod);
            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            var testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().NotContain("Rubbish Scope");
            testString.Should().Contain(SessionId);
            testString.Should().Contain(MethodLogPrefix + "TaskedMethod");

            // Read 2nd log line
            testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain($"{SessionId}=>Rubbish Scope |");
            testString.Should().Contain("Message with rubbish scope");

            streamReader.ReadLine().Should().BeNull();
        }

        [TestMethod]
        public void LogFromNonControllerMethod()
        {
            _systemUnderTest.NonControllerMethod();

            _stream.Position = 0;
            var testString = new StreamReader(_stream).ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain(SessionId);
            testString.Should().Contain(MethodLogPrefix + "NonControllerMethod");
        }

        [TestMethod]
        public void LogInNestedCall()
        {
            Task.Run(_systemUnderTest.NestedCallMethod).Wait();

            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            // Read first log line
            var testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain(SessionId);
            testString.Should().Contain(MethodLogPrefix + "TaskedMethod");

            // Read 2nd log line
            testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain(SessionId);
            testString.Should().Contain(MethodLogPrefix + "NestedCallMethod");
        }

        [TestMethod]
        public void LogExceptionMethod()
        {
            RunControllerMethod(_systemUnderTest.ControllerCatchExceptionMethod);

            _stream.Position = 0;
            var testString = new StreamReader(_stream).ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain(SessionId);
            testString.Should().EndWith("| Logging an exception. [Exception: System.ArgumentException: Something has gone wrong!");
        }

        [TestMethod]
        public void LogSensorFilterMethod()
        {
            RunControllerMethod(_systemUnderTest.ControllerLogSensorFilterMethod);

            _stream.Position = 0;
            var streamReader = new StreamReader(_stream);

            var testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().NotContain("sensitive");

            testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().NotContain("Asterix");
            testString.Should().NotContain("prohibited");
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}
