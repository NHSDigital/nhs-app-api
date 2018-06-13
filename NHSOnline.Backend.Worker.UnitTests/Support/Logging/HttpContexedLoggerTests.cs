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
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Support.Logging;
using System.Threading;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Logging
{
    [TestClass]
    public class HttpContexedLoggerTests
    {
        const string sessionId = "sfd-bnfg-sdf-dsgd-gf";
        const string MethodLogPrefix = "Test log ";

        private class NestedCallClass
        {
            private readonly ILogger _logger;
            public bool PauseNestedExecution { get; set; }

            public NestedCallClass(ILoggerFactory logProvider)
            {
                _logger = logProvider.CreateLogger<DummyController>();
            }

            public async Task TaskAsyncMethod()
            {
                await TaskedMethod();
            }

            private Task<int> TaskedMethod()
            {
                while(PauseNestedExecution)
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
        }

        private IFixture _fixture;
        private DummyController _systemUnderTest;
        private Stream _stream;

        ActionExecutingContext actionExecutingContext;
        ResultExecutedContext resultContext;
        
        [TestInitialize]
        public void TestInitialise()
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
            var actionContext = new ActionContext(new DefaultHttpContext(), new Microsoft.AspNetCore.Routing.RouteData(), new ControllerActionDescriptor());
            actionContext.HttpContext.Items.Add("UserSession", new EmisUserSession() { Key = sessionId });
            _systemUnderTest.ControllerContext = new ControllerContext(actionContext);
            actionExecutingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<String,object>(), _systemUnderTest);
            resultContext = new ResultExecutedContext(actionContext, new List<IFilterMetadata>(), new ObjectResult(1), _systemUnderTest);
        }

        private void ConfigureLogging(ILoggingBuilder logBuilder)
        {
            _stream = new MemoryStream();
            logBuilder.AddProvider(new HttpContexedLoggerProvider(new StreamWriter(_stream), LogLevel.Critical));
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
            attribute.OnActionExecuting(actionExecutingContext);
            controllerMethod();
            attribute.OnActionExecuted(null);// Method should do nothing so can pass null in...
            attribute.OnResultExecuting(null);// Method should do nothing so can pass null in...
            attribute.OnResultExecuted(resultContext);
            return controllerMethod.Method.Name;
        }
        
        [TestMethod]
        public void LogFromControllerMethod()
        {
            var methodName = RunControllerMethod(_systemUnderTest.ControllerMethod);

            _stream.Position = 0;
            var testString = new StreamReader(_stream).ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain(sessionId);
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
            testString.Should().Contain(sessionId);
            testString.Should().Contain(MethodLogPrefix + "TaskedMethod");

            // Read 2nd log line
            testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain($"{sessionId}=>Rubbish Scope |");
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
            testString.Should().Contain(sessionId);
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
            testString.Should().Contain(sessionId);
            testString.Should().Contain(MethodLogPrefix + "TaskedMethod");

            // Read 2nd log line
            testString = streamReader.ReadLine();
            testString.Should().NotBeEmpty();
            testString.Should().Contain(sessionId);
            testString.Should().Contain(MethodLogPrefix + "NestedCallMethod");
        }
    }
}
