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
    public class SecurityModeFilterInvalidRunModeTests
    {
        private SecurityModeFilter _securityModeFilter;
        private IFixture _fixture;
        private Mock<Microsoft.Extensions.Configuration.IConfiguration> _configuration;
        private CidSecurityModeAttribute _cidSecurityModeAttribute;

        [TestInitialize]
        public void TestInitializeInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _cidSecurityModeAttribute = new CidSecurityModeAttribute();
            _configuration = _fixture.Create<Mock<Microsoft.Extensions.Configuration.IConfiguration>>();
            _securityModeFilter = _fixture.Create<SecurityModeFilter>();
            
        }

        [TestMethod]
        public void FilterTest_LoadCidEndPoint_InvalidParam()
        {
            _configuration.SetupGet(x => x["runMode"]).Returns("");
            _fixture.Inject(_configuration.Object);
            var filtdesc = new FilterDescriptor(_cidSecurityModeAttribute, 20);
            var actionExecutingContext = FilterTestUtils.CreateActionExecutingContext(filtdesc);
            _securityModeFilter.OnActionExecuting(actionExecutingContext);
            var result = actionExecutingContext.Result;
            result.Should().BeAssignableTo<NotFoundResult>();
        }
        
        [TestMethod]
        public void FilterTest_LoadCidEndPoint_noRunModeParam()
        {
            _fixture.Inject(_configuration.Object);
            _securityModeFilter = _fixture.Create<SecurityModeFilter>();
            var filtdesc = new FilterDescriptor(_cidSecurityModeAttribute, 20);
            var actionExecutingContext = FilterTestUtils.CreateActionExecutingContext(filtdesc);
            _securityModeFilter.OnActionExecuting(actionExecutingContext);
            var result = actionExecutingContext.Result;
            result.Should().BeAssignableTo<NotFoundResult>();
        }
        
        [TestMethod]
        public void FilterTest_LoadCidEndPoint_incorrectRunModeParam()
        {
            _configuration.SetupGet(x => x["runMode"]).Returns("AAAA");
            _fixture.Inject(_configuration.Object);
            var filtdesc = new FilterDescriptor(_cidSecurityModeAttribute, 20);
            var actionExecutingContext = FilterTestUtils.CreateActionExecutingContext(filtdesc);
            _securityModeFilter.OnActionExecuting(actionExecutingContext);
            var result = actionExecutingContext.Result;
            result.Should().BeAssignableTo<NotFoundResult>();
        }
    }
}
