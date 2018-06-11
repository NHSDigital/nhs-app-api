using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Models
{
    [TestClass]
    public class AuthenticateReplyTests: XmlTestBase<AuthenticateReply>
    {
        protected override AuthenticateReply CreateModel() => new AuthenticateReply
        {
            User = new User()
        };

        [TestMethod]
        public void Serialization_User_SerializesAsElement()
        {
            Element.Should().HaveElement("User");
        }
    }
}