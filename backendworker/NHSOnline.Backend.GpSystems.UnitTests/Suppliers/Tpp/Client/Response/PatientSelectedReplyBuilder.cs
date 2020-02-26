using NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client.Response;
using System.Xml;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client
{
    internal sealed class PatientSelectedReplyBuilder
    {
        private PersonBuilder _person = new PersonBuilder();

        internal string BuildXml() => new TppXmlResponseBuilder("PatientSelectedReply").BuildXml(WriteXml);

        private void WriteXml(XmlWriter writer)
        {
            _person.WriteXml(writer);
        }

        internal PatientSelectedReplyBuilder PatientId(string patientId)
        {
            _person.PatientId(patientId);
            return this;
        }

        internal PatientSelectedReplyBuilder DateOfBirth(string dateOfBirth)
        {
            _person.DateOfBirth(dateOfBirth);
            return this;
        }

        internal PatientSelectedReplyBuilder Gender(string gender)
        {
            _person.Gender(gender);
            return this;
        }

        internal PatientSelectedReplyBuilder NationalIdType(string type)
        {
            _person.NationalIdType(type);
            return this;
        }

        internal PatientSelectedReplyBuilder NationalIdValue(string value)
        {
            _person.NationalIdValue(value);
            return this;
        }

        internal PatientSelectedReplyBuilder PersonName(string name)
        {
            _person.PersonName(name);
            return this;
        }

        internal PatientSelectedReplyBuilder TppAddress(string address)
        {
            _person.Address(address);
            return this;
        }

        internal PatientSelectedReplyBuilder TppAddressType(string type)
        {
            _person.AddressType(type);
            return this;
        }
    }
}