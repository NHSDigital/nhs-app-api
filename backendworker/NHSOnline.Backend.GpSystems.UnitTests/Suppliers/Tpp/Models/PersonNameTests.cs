using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Models
{
    [TestClass]
    public class PersonNameTests: XmlTestBase<PersonName>
    {
        private const string Name = "Homer";

        protected override PersonName CreateModel() => new PersonName
        {
            Name = Name,
        };

        [TestMethod]
        public void Serialization_Name_SerializesAsAttribute()
        {
            Element.Attribute("name").Should().HaveValue(Name);
        }
    }
}