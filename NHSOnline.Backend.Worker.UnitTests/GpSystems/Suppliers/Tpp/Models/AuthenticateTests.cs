using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Models
{
    [TestClass]
    public class AuthenticateTests: XmlTestBase<Authenticate>
    {
        private const string AccountId = "1234";
        private const string ApiVersion = "5678";
        private const string Passphrase = "foo";
        
        protected override Authenticate CreateModel() => new Authenticate
        {
            AccountId = AccountId,
            ApiVersion = ApiVersion,
            Passphrase = Passphrase,
            Application = new Application()
        };

        [TestMethod]
        public void Serialization_AccountId_SerializesAsAttribute()
        {
            Element.Attribute("accountId").Should().HaveValue(AccountId);
        }
        
        [TestMethod]
        public void Serialization_ApiVersion_SerializesAsAttribute()
        {
            Element.Attribute("apiVersion").Should().HaveValue(ApiVersion);
        }
        
        [TestMethod]
        public void Serialization_Passphrase_SerializesAsAttribute()
        {
            Element.Attribute("passphrase").Should().HaveValue(Passphrase);
        }

        [TestMethod]
        public void Serialization_Application_SerializesAsElement()
        {
            Element.Should().HaveElement("Application");
        }
    }
}