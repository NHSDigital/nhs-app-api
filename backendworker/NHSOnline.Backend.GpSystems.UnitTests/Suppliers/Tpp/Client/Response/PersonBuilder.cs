using System.Xml;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client.Response
{
    class PersonBuilder
    {
        private string _patientId = "Patient Id";
        private string _dateOfBirth = "2020-02-20";
        private string _gender = "Gender";
        private NationalIdBuilder _nationalId = new NationalIdBuilder();
        private PersonNameBuilder _personName = new PersonNameBuilder();
        private TppAddressBuilder _address = new TppAddressBuilder();

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Person");
            writer.WriteAttributeString("patientId", _patientId);
            writer.WriteAttributeString("dateOfBirth", _dateOfBirth);
            writer.WriteAttributeString("gender", _gender);
            _nationalId.WriteXml(writer);
            _personName.WriteXml(writer);
            _address.WriteXml(writer);
            writer.WriteEndElement();
        }

        internal PersonBuilder PatientId(string patientId)
        {
            _patientId = patientId;
            return this;
        }

        internal PersonBuilder DateOfBirth(string dateOfBirth)
        {
            _dateOfBirth = dateOfBirth;
            return this;
        }

        internal PersonBuilder Gender(string gender)
        {
            _gender = gender;
            return this;
        }

        internal PersonBuilder NationalIdType(string type)
        {
            _nationalId = _nationalId.Type(type);
            return this;
        }

        internal PersonBuilder NationalIdValue(string value)
        {
            _nationalId = _nationalId.Value(value);
            return this;
        }

        internal PersonBuilder PersonName(string name)
        {
            _personName = _personName.PersonName(name);
            return this;
        }

        internal PersonBuilder Address(string address)
        {
            _address = _address.TppAdress(address);
            return this;
        }

        internal PersonBuilder AddressType(string addressType)
        {
            _address = _address.TppAdressType(addressType);
            return this;
        }
    }
}