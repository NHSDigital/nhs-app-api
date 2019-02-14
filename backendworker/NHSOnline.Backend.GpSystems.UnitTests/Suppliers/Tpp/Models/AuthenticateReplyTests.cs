using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Models
{
    [TestClass]
    public class AuthenticateReplyTests: XmlTestBase<AuthenticateReply>
    {
        private const string PatientId = "2367";
        private const string OnlineUserId = "7422";
        private readonly Guid _uuid = new Guid("2714afa8-89f3-4daa-adf1-7699db85267b");
        
        protected override AuthenticateReply CreateModel() => new AuthenticateReply
        {
            PatientId = PatientId,
            OnlineUserId = OnlineUserId,
            User = new User(),
            Uuid = _uuid
        };

        [TestMethod]
        public void Serialization_PatientId_SerializesAsAttribute()
        {
            Element.Attribute("patientId").Should().HaveValue(PatientId);
        }
        
        [TestMethod]
        public void Serialization_OnlineUserId_SerializesAsAttribute()
        {
            Element.Attribute("onlineUserId").Should().HaveValue(OnlineUserId);
        }

        [TestMethod]
        public void Serialization_Uuid_SerializesAsAttribute()
        {
            Element.Attribute("uuid").Should().HaveValue(_uuid.ToString());
        }

        [TestMethod]
        public void Serialization_User_SerializesAsElement()
        {
            Element.Should().HaveElement("User");
        }
    }
}