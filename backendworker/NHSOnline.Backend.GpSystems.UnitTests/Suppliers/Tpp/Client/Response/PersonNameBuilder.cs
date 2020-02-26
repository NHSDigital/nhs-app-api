using System.Xml;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client.Response
{
    internal sealed class PersonNameBuilder
    {
        private string _name = "Name";

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("PersonName");
            writer.WriteAttributeString("name", _name);
            writer.WriteEndElement();
        }

        internal PersonNameBuilder PersonName(string name)
        {
            _name = name;
            return this;
        }
    }
}