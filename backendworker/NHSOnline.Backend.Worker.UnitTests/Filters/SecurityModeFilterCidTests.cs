using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Filters;

namespace NHSOnline.Backend.Worker.UnitTests.Filters
{
    [TestClass]
    public class SecurityModeFilterCidTest
    {
        private SecurityModeFilter _securityModeFilter;
        private IFixture _fixture;
        private Mock<Microsoft.Extensions.Configuration.IConfiguration> _configuration;
        private CidSecurityModeAttribute _cidSecurityModeAttribute;
        private PfsSecurityModeAttribute _pfsSecurityModeAttribute;

        [TestInitialize]
        public void TestInitializeInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _cidSecurityModeAttribute = new CidSecurityModeAttribute();
            _pfsSecurityModeAttribute = new PfsSecurityModeAttribute();
            _configuration = _fixture.Create<Mock<Microsoft.Extensions.Configuration.IConfiguration>>();
            _configuration.SetupGet(x => x["runMode"]).Returns("cid");
            _fixture.Inject(_configuration.Object);
            _securityModeFilter = _fixture.Create<SecurityModeFilter>();
        }

        [TestMethod]
        public void FilterTest_constructorTest()
        {
            Assert.AreEqual(RunMode.Cid, _cidSecurityModeAttribute.SecurityMode);
        }

        [TestMethod]
        public void FilterTest_LoadCidEndPoint_CidParam()
        {
            var filtdesc = new FilterDescriptor(_cidSecurityModeAttribute, 20);
            var actionExecutingContext = FilterTestUtils.CreateActionExecutingContext(filtdesc);
            _securityModeFilter.OnActionExecuting(actionExecutingContext);
            Assert.IsNull(actionExecutingContext.Result);
        }

        [TestMethod]
        public void FilterTest_LoadPfsEndPoint_CidParam()
        {
            var filtdesc = new FilterDescriptor(_pfsSecurityModeAttribute, 20);
            var actionExecutingContext = FilterTestUtils.CreateActionExecutingContext(filtdesc);
            _securityModeFilter.OnActionExecuting(actionExecutingContext);
            var result = actionExecutingContext.Result;
            result.Should().BeAssignableTo<NotFoundResult>();
        }
    }
}
