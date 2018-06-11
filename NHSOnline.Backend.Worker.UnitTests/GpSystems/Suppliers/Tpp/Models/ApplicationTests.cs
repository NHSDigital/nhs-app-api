using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Models
{
    [TestClass]
    public class ApplicationTests: XmlTestBase<Application>
    {
        private const string NhsApp = "NHS App";
        private const string DeviceType = "moble";
        private const string ProviderId = "1234";
        private const string Version = "5";

        protected override Application CreateModel() => new Application
        {
            Name = NhsApp,
            DeviceType = DeviceType,
            ProviderId = ProviderId,
            Version = Version
        };
        
        [TestMethod]
        public void Serialization_Name_SerializesAsAttribute()
        {
            Element.Attribute("name").Should().HaveValue(NhsApp);
        }
        
        [TestMethod]
        public void Serialization_DeviceType_SerializesAsAttribute()
        {
            Element.Attribute("deviceType").Should().HaveValue(DeviceType);
        }
        
        [TestMethod]
        public void Serialization_ProviderId_SerializesAsAttribute()
        {
            Element.Attribute("providerId").Should().HaveValue(ProviderId);
        }
        
        [TestMethod]
        public void Serialization_Version_SerializesAsAttribute()
        {
            Element.Attribute("version").Should().HaveValue(Version);
        }
    }
}