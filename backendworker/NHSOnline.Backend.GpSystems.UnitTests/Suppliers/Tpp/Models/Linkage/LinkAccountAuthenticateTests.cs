using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Models
{
    [TestClass]
    public class LinkAccountAuthenticateTests : XmlTestBase<LinkAccountAuthenticate>
    {
        private const string ApiVersion = "5678";
        private const string LastName = "bar";
        private const string OrganisationCode = "ods";
        private readonly DateTime _dateOfBirth = new DateTime(2008, 3, 9, 16, 5, 7);
        private readonly Guid _uuid = new Guid("9b45b59f-96a1-41b1-8c69-7664c9f998ea");
        private const string LinkagePassphrase = "foo";
        private const string AccountId = "1234";

        protected override LinkAccountAuthenticate CreateModel() => new LinkAccountAuthenticate
        {
            ApiVersion = ApiVersion,
            LastName = LastName,
            DateofBirth = _dateOfBirth,
            OrganisationCode = OrganisationCode,
            Uuid = _uuid,
            Application = new Application(),
            Passphrase = LinkagePassphrase,
            AccountId = AccountId,
        };

        [TestMethod]
        public void Serialization_NameShouldBeCorrect()
        {
            Element.Name.LocalName.Should().Be("LinkAccount");
        }

        [TestMethod]
        public void Serialization_ApiVersion_SerializesAsAttribute()
        {
            Element.Attribute("apiVersion").Should().HaveValue(ApiVersion);
        }

        [TestMethod]
        public void Serialization_LastName_SerializesAsAttribute()
        {
            Element.Attribute("lastName").Should().HaveValue(LastName);
        }

        [TestMethod]
        public void Serialization_OrganisationCode_SerializesAsAttribute()
        {
            Element.Attribute("organisationCode").Should().HaveValue(OrganisationCode);
        }

        [TestMethod]
        public void Serialization_DateOfBirth_SerializesAsAttribute()
        {
            var dateStringValue = String.Format(CultureInfo.InvariantCulture, "{0:s}", _dateOfBirth);
            Element.Attribute("dateOfBirth").Should().HaveValue(dateStringValue);
        }

        [TestMethod]
        public void Serialization_AccountId_SerializesAsAttribute()
        {
            Element.Attribute("accountId").Should().HaveValue(AccountId);
        }

        [TestMethod]
        public void Serialization_Passphrase_SerializesAsAttribute()
        {
            Element.Attribute("passphrase").Should().HaveValue(LinkagePassphrase);
        }

        [TestMethod]
        public void Serialization_Uuid_SerializesAsAttribute()
        {
            Element.Attribute("uuid").Should().HaveValue(_uuid.ToString());
        }

        [TestMethod]
        public void Serialization_Application_SerializesAsElement()
        {
            Element.Should().HaveElement("Application");
        }
    }
}
