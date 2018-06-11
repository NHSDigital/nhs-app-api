using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Models
{
    [TestClass]
    public class PersonTests: XmlTestBase<Person>
    {
        private const string Gender = "Female";
        private const string PatientId = "654";
        private readonly DateTime _dateOfBirth = new DateTime(1980, 1, 4);

        protected override Person CreateModel() => new Person
        {
            DateOfBirth = _dateOfBirth,
            Gender = Gender,
            PatientId = PatientId,
            NationalId = new NationalId(),
            PersonName = new PersonName()
        };

        [TestMethod]
        public void Serialization_DateOfBirth_SerializesAsAttribute()
        {
            Element.Attribute("dateOfBirth")?.Value.Should().NotBeNull();
        }

        [TestMethod]
        public void Serialization_Gender_SerializesAsAttribute()
        {
            Element.Attribute("gender").Should().HaveValue(Gender);
        }
        
        [TestMethod]
        public void Serialization_PatientId_SerializesAsAttribute()
        {
            Element.Attribute("patientId").Should().HaveValue(PatientId);
        }
        
        [TestMethod]
        public void Serialization_NationalId_SerializesAsElement()
        {
            Element.Should().HaveElement("NationalId");
        }
        
        [TestMethod]
        public void Serialization_PersonName_SerializesAsElement()
        {
            Element.Should().HaveElement("PersonName");
        }
    }
}