using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Models
{
    [TestClass]
    public class NationalIdTests: XmlTestBase<NationalId>
    {
        private const string Type = "a number";
        private const string Value = "1 2 3";

        protected override NationalId CreateModel() => new NationalId
        {
            Type = Type,
            Value = Value
        };

        [TestMethod]
        public void Serialization_Type_SerializesAsAttribute()
        {
            Element.Attribute("type").Should().HaveValue(Type);
        }

        [TestMethod]
        public void Serialization_Value_SerializesAsValue()
        {
            Element.Should().HaveValue(Value);
        }
    }
}