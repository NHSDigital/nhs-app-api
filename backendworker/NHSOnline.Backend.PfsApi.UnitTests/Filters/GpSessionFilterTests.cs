using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.PfsApi.GpSession;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Filters
{
    [TestClass]
    [SuppressMessage("ReSharper", "CA1801", Justification = "Test controller methods do nothing")]
    public class GpSessionFilterTests
    {
        private ILogger<GpSessionFilter> _logger;
        private IUserSessionService _userSessionService;
        private IGpSessionCreator _gpSessionCreator;
        private IGpSystem _gpSystem;
        private int _responseStatusCode;
        private PfsErrorResponse _errorResponse;
        private ISessionErrorResultBuilder _errorResultBuilder;
        private ILinkedAccountsService _linkedAccountsService;

        private GpSessionFilter _gpSessionFilter;

        private IDictionary<string, object> _actionArguments;

        private ActionExecutingContext _context;
        private bool _nextCalled;
        private ActionExecutionDelegate _next;

        private P5UserSession _p5UserSession;
        private GpUserSession _gpUserSession;
        private P9UserSession _p9UserSession;

        private UserSession _userSession;

        private GpSessionRecreateResult _sessionRecreateResult;
        private GpSessionRecreateResult _sessionRecreateErrorResult;

        private List<LinkedAccount> _linkedAccounts;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private HeaderDictionary _headers;

        public static void MethodWithCorrectGpSessionTypeAndAttribute([GpSession] GpUserSession gpUserSession)
        {
        }

        public static void MethodWithMultipleCorrectGpSessionTypesAndAttributes(
            [GpSession] GpUserSession gpUserSession1,
            [GpSession] GpUserSession gpUserSession2)
        {
        }

        public static void MethodWithCorrectGpSessionTypeAndNoAttribute(GpUserSession gpUserSession)
        {
        }

        public static void MethodWithIgnoreP5UserAttribute(
            [GpSession(IgnoreP5Users = true)] GpUserSession gpUserSession,
            Guid patientId)
        {
        }

        [TestInitialize]
        public void Setup()
        {
            // mock dependencies
            _logger = Mock.Of<ILogger<GpSessionFilter>>();
            _userSessionService = Mock.Of<IUserSessionService>();

            _responseStatusCode = 599;
            _errorResponse = new PfsErrorResponse()
            {
                ServiceDeskReference = "z8wuejh"
            };

            _errorResultBuilder = Mock.Of<ISessionErrorResultBuilder>();

            Mock.Get(_errorResultBuilder)
                .Setup(b => b.BuildResult(It.IsAny<ErrorTypes>()))
                .Returns(() =>
                    new ObjectResult(_errorResponse)
                    {
                        StatusCode = _responseStatusCode
                    }
                );

            _linkedAccounts = new List<LinkedAccount>();
            _linkedAccountsService = Mock.Of<ILinkedAccountsService>();

            Mock.Get(_linkedAccountsService)
                .Setup(s => s.GetLinkedAccounts(It.IsAny<GpUserSession>(), It.IsAny<Dictionary<Guid,string>>()))
                .Returns(() =>
                    Task.FromResult<LinkedAccountsResult>(
                        new LinkedAccountsResult.Success(_linkedAccounts, false)));

            _gpSystem = Mock.Of<IGpSystem>();

            Mock.Get(_gpSystem)
                .Setup(s => s.GetLinkedAccountsService())
                .Returns(() => _linkedAccountsService);

            var gpSystemFactory = Mock.Of<IGpSystemFactory>();

            Mock.Get(gpSystemFactory)
                .Setup(s => s.CreateGpSystem(It.IsAny<Supplier>()))
                .Returns(() => _gpSystem);

            _gpSessionCreator = Mock.Of<IGpSessionCreator>();

            Mock.Get(_gpSessionCreator)
                .Setup(r => r.RecreateGpSession(It.IsAny<P9UserSession>(), It.IsAny<Supplier>(), It.IsAny<Guid>()))
                .Returns(() =>
                {
                    SetupGpSession(); // mock recreating the session

                    _p9UserSession.GpUserSession = _gpUserSession;

                    return Task.FromResult(_sessionRecreateResult);
                });

            Mock.Get(_userSessionService)
                .Setup(s => s.GetUserSession<P9UserSession>())
                .Returns(() =>
                {
                    if (_userSession is null || _userSession == _p5UserSession)
                    {
                        return Option.None<P9UserSession>(); // mock user session logic
                    }

                    return Option.Some(_userSession as P9UserSession);
                });

            _sessionRecreateResult = new GpSessionRecreateResult.RecreatedResult();

            _sessionRecreateErrorResult = new GpSessionRecreateResult.ErrorResult(new ErrorTypes.UnhandledError(), "error");

            _headers = new HeaderDictionary();

            // mock return values
            _p5UserSession = new P5UserSession(Guid.NewGuid().ToString(), new CitizenIdUserSession());

            UseP9Session();

            // method params
            _actionArguments = new Dictionary<string, Object>();

            var requestMock = new Mock<HttpRequest>();
            requestMock
                .SetupGet(x => x.Headers)
                .Returns(_headers);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock
                .SetupGet(x => x.Request)
                .Returns(requestMock.Object);

            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _context = new ActionExecutingContext(
                new ActionContext(
                    httpContextMock.Object,
                    Mock.Of<RouteData>(),
                    Mock.Of<ActionDescriptor>()
                ),
                new List<IFilterMetadata>(),
                _actionArguments,
                this);

            BuildEndpointParameters(nameof(MethodWithCorrectGpSessionTypeAndAttribute));

            _nextCalled = false;
            _next = () =>
            {
                _nextCalled = true;
                return Task.FromResult<ActionExecutedContext>(null);
            };

            // system under test
            _gpSessionFilter = new GpSessionFilter(
                _logger,
                _userSessionService,
                _gpSessionCreator,
                _errorResultBuilder,
                _httpContextAccessor.Object);
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithValidP9Session_ThenGpSessionIsInjectedIntoEndpointParameters()
        {
            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Assert.IsTrue(_actionArguments.ContainsKey("gpUserSession"));
            Assert.AreEqual(_gpUserSession, _actionArguments["gpUserSession"]);
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithValidP9Session_ThenGpSessionIsInjectedIntoMultipleEndpointParameters()
        {
            BuildEndpointParameters(nameof(MethodWithMultipleCorrectGpSessionTypesAndAttributes));

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Assert.IsTrue(_actionArguments.ContainsKey("gpUserSession1"));
            Assert.AreEqual(_gpUserSession, _actionArguments["gpUserSession1"]);

            Assert.IsTrue(_actionArguments.ContainsKey("gpUserSession2"));
            Assert.AreEqual(_gpUserSession, _actionArguments["gpUserSession2"]);
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithValidP9Session_ThenNextMiddlewareIsCalled()
        {
            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Assert.IsTrue(_nextCalled);
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithP5Session_Then401StatusCodeResultIsReturned()
        {
            UseP5Session();

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Assert.AreEqual(
                typeof(UnauthorizedResult),
                _context.Result.GetType());
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithP5SessionIgnored_ThenNextMiddlewareIsCalled()
        {
            UseP5Session();

            BuildEndpointParameters(nameof(MethodWithIgnoreP5UserAttribute));

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Assert.IsTrue(_nextCalled);
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithP5Session_ThenNextMiddlewareIsNotCalled()
        {
            UseP5Session();

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Assert.IsFalse(_nextCalled);
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithInvalidP9Session_AndSessionRecreateReturnsAnError_ThenNextMiddlewareIsNotCalled()
        {
            UseP9Session(false);

            _sessionRecreateResult = _sessionRecreateErrorResult;

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Assert.IsFalse(_nextCalled);
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithInvalidP9Session_AndSessionRecreateReturnsAnError_ThenGpUnavailableErrorPassedToResponseBuilder()
        {
            UseP9Session(false);

            _sessionRecreateResult = _sessionRecreateErrorResult;

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Mock.Get(_errorResultBuilder)
                .Verify(b => b.BuildResult(It.IsAny<ErrorTypes.GPSessionUnavailable>()));
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithInvalidP9Session_AndSessionRecreateReturnsAnError_ThenGpUnavailableErrorPassedToResponseBuilderHasCorrectErrorDetails()
        {
            UseP9Session(false);

            _sessionRecreateResult = _sessionRecreateErrorResult;

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            var errorCaptor = new ArgumentCaptor<ErrorTypes.GPSessionUnavailable>();

            Mock.Get(_errorResultBuilder)
                .Verify(b => b.BuildResult(errorCaptor.Capture()));

            Assert.AreEqual(ErrorCategory.None, errorCaptor.Value.Category);
            Assert.AreEqual("xx", errorCaptor.Value.Prefix);
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithInvalidP9Session_AndSessionRecreateReturnsAnError_ThenContextResultHasStatusCode()
        {
            UseP9Session(false);

            _sessionRecreateResult = _sessionRecreateErrorResult;

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            var result = _context.Result as ObjectResult;

            Assert.AreEqual(_responseStatusCode, result.StatusCode);
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithInvalidP9Session_AndSessionRecreateReturnsAnError_ThenContextResultHasObject()
        {
            UseP9Session(false);

            _sessionRecreateResult = _sessionRecreateErrorResult;

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            var result = _context.Result as ObjectResult;

            Assert.AreEqual(
                _errorResponse,
                result.Value);
        }

        //new PfsErrorResponse { ServiceDeskReference = serviceDeskReference }

        [DataTestMethod]
        public async Task WhenFilterInvoked_WithValidP9Session_AndGpSessionIsInvalid_ThenSessionIsRecreated()
        {
            UseP9Session(false, Supplier.Unknown);

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Mock.Get(_gpSessionCreator)
                .Verify(v => v.RecreateGpSession(It.IsAny<P9UserSession>(), It.IsAny<Supplier>(), It.IsAny<Guid>()));
        }

        [DataTestMethod]
        [DataRow(false, Supplier.Unknown)]
        [DataRow(true, Supplier.Unknown)]
        public async Task
            WhenFilterInvoked_WithValidP9Session_AndGpSessionIsInvalid_ThenGpSessionIsInjectedIntoEndpointParameters(
                bool hasGpSupplier, Supplier supplier)
        {
            UseP9Session(hasGpSupplier, supplier);
            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Assert.IsTrue(_actionArguments.ContainsKey("gpUserSession"));
            Assert.AreEqual(_gpUserSession, _actionArguments["gpUserSession"]);
        }

        [DataTestMethod]
        [DataRow(false, Supplier.Unknown)]
        [DataRow(true, Supplier.Unknown)]
        public async Task WhenFilterInvoked_WithValidP9Session_AndGpSessionIsInvalid_ThenNextMiddlewareIsCalled(
            bool hasGpSupplier, Supplier supplier)
        {
            UseP9Session(hasGpSupplier, supplier);

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Assert.IsTrue(_nextCalled);
        }

        [TestMethod]
        public async Task WhenFilterInvoked_WithNoEndpointParametersWithAttribute_ThenNextMiddlewareIsCalled()
        {
            BuildEndpointParameters(nameof(MethodWithCorrectGpSessionTypeAndNoAttribute));

            await _gpSessionFilter.OnActionExecutionAsync(_context, _next);

            Assert.IsTrue(_nextCalled);
        }

        private void BuildEndpointParameters(string testMethodName)
        {
            var endpointParameters = this.GetType()
                .GetMethods()
                .First(m => m.IsStatic && m.Name == testMethodName)
                .GetParameters()
                .Select(p =>
                    new ControllerParameterDescriptor
                    {
                        ParameterType = p.ParameterType,
                        ParameterInfo = p,
                        Name = p.Name
                    }).ToList<ParameterDescriptor>();

            _context.ActionDescriptor = new ActionDescriptor
            {
                Parameters = endpointParameters
            };
        }

        private void UseP5Session()
        {
            _userSession = _p5UserSession;
        }

        private void UseP9Session(bool hasGpSession = true, Supplier supplier = Supplier.Emis)
        {
            if (hasGpSession)
            {
                SetupGpSession(supplier);
            }
            else
            {
                _gpUserSession = new NullGpSession(supplier, "_ref");
            }

            _p9UserSession = new P9UserSession(
                Guid.NewGuid().ToString(),
                "1234567890",
                new CitizenIdUserSession(),
                Guid.NewGuid().ToString(), _gpUserSession);
            _headers[Constants.HttpHeaders.PatientId] = new StringValues(_p9UserSession.PatientSessionId.ToString());
            _userSession = _p9UserSession;
        }

        private void SetupGpSession(Supplier supplier = Supplier.Emis)
        {
            _gpUserSession = new MockGpUserSession
            {
                HasLinkedAccountsValue = _linkedAccounts.Any(),
                SupplierValue = supplier
            };
        }

        private void BuildPatientIdEndpointParameters(string testMethodName)
        {
            var endpointParameters = this.GetType()
                .GetMethods()
                .First(m => m.IsStatic && m.Name == testMethodName)
                .GetParameters()
                .Select(p =>
                    new ControllerParameterDescriptor
                    {
                        ParameterType = p.ParameterType,
                        ParameterInfo = p,
                        Name = p.Name
                    }).ToList<ParameterDescriptor>();

            _context.ActionDescriptor = new ActionDescriptor
            {
                Parameters = endpointParameters
            };
        }

        private void SetupLinkedProfiles()
        {
            _linkedAccounts = new List<LinkedAccount>
            {
                new LinkedAccount
                {
                    Id = Guid.NewGuid()
                },
                new LinkedAccount
                {
                    Id = Guid.NewGuid()
                }
            };
        }
    }
}
