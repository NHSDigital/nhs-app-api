using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Session
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
        public async Task Extend_ReturnsSuccess()
        {
            var result = await _systemUnderTest.Extend(_visionUserSession);

            result.Should().BeAssignableTo<SessionExtendResult.Success>();
        }
    }
}
