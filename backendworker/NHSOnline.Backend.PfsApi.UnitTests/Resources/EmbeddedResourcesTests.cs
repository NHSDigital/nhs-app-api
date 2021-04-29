using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.Resources;

namespace NHSOnline.Backend.PfsApi.UnitTests.Resources
{
    [TestClass]
    public class EmbeddedResourcesTests
    {
        [TestMethod]
        public void GetEmbeddedResource_EmptyString_ThrowsException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => EmbeddedResources.GetEmbeddedResource(string.Empty));
        }

        [TestMethod]
        public void GetEmbeddedResource_Null_ThrowsException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => EmbeddedResources.GetEmbeddedResource(null));
        }

        [TestMethod]
        public void GetEmbeddedResource_InvalidValue_ThrowsException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => EmbeddedResources.GetEmbeddedResource("InvalidValue"));
        }

        [TestMethod]
        public void GetEmbeddedResource_ValidValue_ReturnsValidOutput()
        {
            var result = EmbeddedResources.GetEmbeddedResource(EmbeddedResources.IntroductoryMessage);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(string.Empty, result);
        }
    }
}
