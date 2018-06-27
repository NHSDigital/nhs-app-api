using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Models
{
    [TestClass]
    public class LinkAccountReplyTests : XmlTestBase<LinkAccountReply>
    {
        private const string Passphrase = "passphrase123";
        private readonly Guid _uuid = new Guid("2714afa8-89f3-4daa-adf1-7699db85267b");
        
        protected override LinkAccountReply CreateModel() => new LinkAccountReply
        {
            Passphrase = Passphrase,
            Uuid = _uuid
        };

        [TestMethod]
        public void Serialization_Passphrase_SerializesAsAttribute()
        {
            Element.Attribute("passphrase").Should().HaveValue(Passphrase);
        }
        
        [TestMethod]
        public void Serialization_Uuid_SerializesAsAttribute()
        {
            Element.Attribute("uuid").Should().HaveValue(_uuid.ToString());
        }
    }
}