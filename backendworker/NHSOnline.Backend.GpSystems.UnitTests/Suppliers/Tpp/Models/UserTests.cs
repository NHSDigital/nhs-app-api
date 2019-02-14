using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Models
{
    [TestClass]
    public class UserTests: XmlTestBase<User>
    {
        protected override User CreateModel() => new User
        {
            Person = new Person()
        };

        [TestMethod]
        public void Serialization_Person_SerializesAsElement()
        {
            Element.Should().HaveElement("Person");
        }
    }
}
