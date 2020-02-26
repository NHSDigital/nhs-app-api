using System.Xml;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client.Response
{
    internal sealed class TppAddressBuilder
    {
        private string _address = "Address";
        private string _addressType = "Address Type";

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Address");
            writer.WriteAttributeString("address", _address);
            writer.WriteAttributeString("addressType", _addressType);
            writer.WriteEndElement();
        }

        internal TppAddressBuilder TppAdress(string address)
        {
            _address = address;
            return this;
        }

        internal TppAddressBuilder TppAdressType(string addressType)
        {
            _addressType = addressType;
            return this;
        }
    }
}