using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Models
{
    [TestClass]
    public class ErrorTests: XmlTestBase<Error>
    {
        private const string ErrorCode = "23";
        private const string UserFriendlyMessage = "This is for the user";
        private const string TechnicalMessage = "This is for the support";
        private readonly Guid _uuid = new Guid("f6c2227e-96aa-41ea-8614-efa6cf81c353");

        protected override Error CreateModel() => new Error
        {
            ErrorCode = ErrorCode,
            UserFriendlyMessage = UserFriendlyMessage,
            TechnicalMessage = TechnicalMessage,
            Uuid = _uuid
        };

        [TestMethod]
        public void Serialization_ErrorCode_SerializesAsAttribute()
        {
            Element.Attribute("errorCode").Should().HaveValue(ErrorCode);
        }
        
        [TestMethod]
        public void Serialization_UserFriendlyMessage_SerializesAsAttribute()
        {
            Element.Attribute("userFriendlyMessage").Should().HaveValue(UserFriendlyMessage);
        }
        
        [TestMethod]
        public void Serialization_TechnicalMessage_SerializesAsAttribute()
        {
            Element.Attribute("technicalMessage").Should().HaveValue(TechnicalMessage);
        }
        
        [TestMethod]
        public void Serialization_Uuid_SerializesAsAttribute()
        {
            Element.Attribute("uuid").Should().HaveValue(_uuid.ToString());
        }
    }
}