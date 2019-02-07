using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Session
{
    [TestClass]
    public class VisionSessionExtendServiceTests
    {
        private IFixture _fixture;        
        private VisionSessionExtendService _systemUnderTest;
        private GpUserSession _visionUserSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _visionUserSession = _fixture.Create<VisionUserSession>();

            _systemUnderTest = _fixture.Create<VisionSessionExtendService>();
        }

        [TestMethod]
        public async Task Extend_ReturnsSuccessfullyExtended()
        {
            var result = await _systemUnderTest.Extend(_visionUserSession);

            result.Should().BeAssignableTo<SessionExtendResult.SuccessfullyExtended>();
        }
    }
}
