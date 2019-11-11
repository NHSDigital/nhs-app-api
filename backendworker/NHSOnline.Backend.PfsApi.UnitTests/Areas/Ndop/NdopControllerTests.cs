using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.Areas.Ndop;
using NHSOnline.Backend.PfsApi.Filters;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Ndop
{
    [TestClass]
    public class NdopControllerTests
    {
        [TestMethod]
        public void EnsureControllerHasProxyingNotAllowedAttribute_ToPreventProxyAccess()
        {
            typeof(NdopController).Should().BeDecoratedWith<ProxyingNotAllowedAttribute>();
        }
    }
}
