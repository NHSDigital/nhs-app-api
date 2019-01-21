using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Session
{
    [TestClass]
    public class VisionSessionExtendServiceTests
    {
        private IFixture _fixture;
        private UserSession _userSession;
        
        private VisionSessionExtendService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<VisionUserSession>()));
            
            _userSession = _fixture.Create<UserSession>();

            _systemUnderTest = _fixture.Create<VisionSessionExtendService>();
        }

        [TestMethod]
        public async Task Extend_ReturnsSuccessfullyExtended()
        {
            var result = await _systemUnderTest.Extend(_userSession);

            result.Should().BeAssignableTo<SessionExtendResult.SuccessfullyExtended>();
        }
    }
}
