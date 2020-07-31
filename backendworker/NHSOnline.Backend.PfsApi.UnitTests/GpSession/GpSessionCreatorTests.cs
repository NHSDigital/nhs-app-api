using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auditing.UnitTestsSupport;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.GpSession;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using GpSessionRecreateResult = NHSOnline.Backend.PfsApi.GpSession.GpSessionRecreateResult;

namespace NHSOnline.Backend.PfsApi.UnitTests.GpSession
{
    [TestClass]
    public class GpSessionCreatorTests
    {
        private IGpSystemFactory _gpSystemFactory;
        private ISessionCacheService _sessionCacheService;
        private IGpSessionManager _sessionManager;
        private IErrorReferenceGenerator _errorReferenceGenerator;

        private GpUserSession _gpSession;
        private P9UserSession _p9UserSession;

        private GpSessionCreateResult _sessionCreateResult;

        private GpSessionCreator _sessionCreator;

        [TestInitialize]
        public void Setup()
        {
            _sessionCacheService = Mock.Of<ISessionCacheService>();

            _gpSession = Mock.Of<GpUserSession>();
            _sessionCreateResult = new GpSessionCreateResult.Success(_gpSession);
            _sessionManager = Mock.Of<IGpSessionManager>();
            _errorReferenceGenerator = Mock.Of<IErrorReferenceGenerator>();

            Mock.Get(_sessionManager)
                .Setup(m => m.CreateSession(It.IsAny<IGpSystem>(), It.IsAny<IGpSessionCreateArgs>()))
                .Returns(() => Task.FromResult(_sessionCreateResult));

            var gpSystem = Mock.Of<IGpSystem>();
            _gpSystemFactory = Mock.Of<IGpSystemFactory>();

            Mock.Get(_gpSystemFactory)
                .Setup(s => s.CreateGpSystem(It.IsAny<Supplier>()))
                .Returns(() => gpSystem);

            _p9UserSession = new P9UserSession(
                Guid.NewGuid().ToString(),
                "0987654321",
                new CitizenIdUserSession
                {
                    OdsCode = "X4545"
                },
                null,
                Guid.NewGuid().ToString()
            );

            _p9UserSession.GpUserSession = new NullGpSession(Supplier.Emis, "_ref");

            var auditor = Mock.Of<IAuditor>();

            Mock.Get(auditor)
                .Setup(a => a.Audit())
                .Returns(new AuditBuilderStub());

            _sessionCreator = new GpSessionCreator(
                Mock.Of<ILogger<GpSessionCreator>>(),
                auditor,
                _gpSystemFactory,
                _sessionManager,
                _sessionCacheService,
                _errorReferenceGenerator);
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_ThenGpSystemIsFetchedForSupplier()
        {
            await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Emis);

            Mock.Get(_gpSystemFactory)
                .Verify(s => s.CreateGpSystem(Supplier.Emis));
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_ThenGpSessionManagerIsCalled()
        {
            await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Emis);

            Mock.Get(_sessionManager)
                .Verify(s => s.CreateSession(It.IsAny<IGpSystem>(), It.IsAny<IGpSessionCreateArgs>()));
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_ThenGpSessionCreateArgsAreCorrect()
        {
            await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Emis);

            var createArgsCaptor = new ArgumentCaptor<IGpSessionCreateArgs>();

            Mock.Get(_sessionManager)
                .Verify(s => s.CreateSession(It.IsAny<IGpSystem>(), createArgsCaptor.Capture()));

            Assert.AreEqual(_p9UserSession.Im1ConnectionToken, createArgsCaptor.Value.Im1ConnectionToken);
            Assert.AreEqual(_p9UserSession.OdsCode, createArgsCaptor.Value.OdsCode);
            Assert.AreEqual(_p9UserSession.NhsNumber, createArgsCaptor.Value.NhsNumber);
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_AndGpSystemReturnsSuccess_ThenUserSessionObjectIsUpdated()
        {
            await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Emis);

            Assert.AreEqual(_gpSession, _p9UserSession.GpUserSession);
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_AndGpSystemReturnsSuccess_ThenUserSessionCacheIsUpdated()
        {
            await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Emis);

            Mock.Get(_sessionCacheService)
                .Verify(s => s.UpdateUserSession(_p9UserSession));
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_AndGpSystemReturnsSuccess_ThenSuccessResultIsReturned()
        {
            var result = await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Emis);

            Assert.AreEqual(
                typeof(GpSessionRecreateResult.RecreatedResult),
                result.GetType());
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_AndGpSystemReturnsFailure_ThenCorrectErrorPrefixIsGenerated()
        {
            _sessionCreateResult = new GpSessionCreateResult.BadGateway("42");

            await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Emis);

            var errorCaptor = new ArgumentCaptor<ErrorTypes>();

            Mock.Get(_errorReferenceGenerator)
                .Verify(g => g.GenerateAndLogErrorReference(errorCaptor.Capture()));

            Assert.AreEqual("3e", errorCaptor.Value.Prefix);
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_AndGpSystemReturnsFailure_ThenErrorResultIsReturned()
        {
            _sessionCreateResult = new GpSessionCreateResult.Timeout("downstream system is asleep");

            var result = await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Emis);

            Assert.AreEqual(
                typeof(GpSessionRecreateResult.ErrorResult),
                result.GetType());
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_AndGpSystemReturnsFailure_ThenErrorResultHasCorrectErrorType()
        {
            _sessionCreateResult = new GpSessionCreateResult.Unparseable("the error due to parsing");

            var result = await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Emis);
            var errorResult = result as GpSessionRecreateResult.ErrorResult;

            Assert.AreEqual(
                typeof(ErrorTypes.LoginGPUnparseable),
                errorResult.ErrorType.GetType());
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_AndGpSystemReturnsFailure_ThenErrorResultHasDetails()
        {
            _sessionCreateResult = new GpSessionCreateResult.InternalServerError("this should come back");

            var result = await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Emis);
            var errorResult = result as GpSessionRecreateResult.ErrorResult;

            Assert.AreEqual(
                "this should come back",
                errorResult.Details);
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_WithUnknownSupplier_ThenErrorResultIsReturned()
        {
            var result = await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Unknown);

            Assert.AreEqual(
                typeof(GpSessionRecreateResult.ErrorResult),
                result.GetType());
        }

        [TestMethod]
        public async Task WhenRecreateGpSessionCalled_WithUnknownSupplier_ThenErrorResultIsOdsCodeNotFound()
        {
            var result = await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Unknown);
            var errorResult = result as GpSessionRecreateResult.ErrorResult;

            Assert.AreEqual(
                typeof(ErrorTypes.LoginOdsCodeNotFoundOrNotSupported),
                errorResult.ErrorType.GetType());
        }

        [TestMethod]
        public async Task WhenRecreateSessionCalled_ThenRecreateResultIsReturned()
        {
            var result = await _sessionCreator.RecreateGpSession(_p9UserSession, Supplier.Emis);

            Assert.AreEqual(
                typeof(GpSessionRecreateResult.RecreatedResult),
                result.GetType());
        }
    }
}
