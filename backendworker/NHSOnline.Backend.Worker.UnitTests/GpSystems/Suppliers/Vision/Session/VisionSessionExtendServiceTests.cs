using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Session
{
    [TestClass]
    public class VisionSessionExtendServiceTests
    {
        private IFixture _fixture;
        private VisionUserSession _userSession;
        private VisionSessionExtendService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _userSession = _fixture.Create<VisionUserSession>();

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
