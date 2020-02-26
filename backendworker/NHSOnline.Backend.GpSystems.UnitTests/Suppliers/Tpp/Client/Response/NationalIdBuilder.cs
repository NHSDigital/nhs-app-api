using System.Xml;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client
{
    internal sealed class NationalIdBuilder
    {
        private string _type = "Type";
        private string _value = "Value";

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("NationalId");
            writer.WriteAttributeString("type", _type);
            writer.WriteString(_value);
            writer.WriteEndElement();
        }

        internal NationalIdBuilder Type(string type)
        {
            _type = type;
            return this;
        }

        internal NationalIdBuilder Value(string value)
        {
            _value = value;
            return this;
        }
    }
}
