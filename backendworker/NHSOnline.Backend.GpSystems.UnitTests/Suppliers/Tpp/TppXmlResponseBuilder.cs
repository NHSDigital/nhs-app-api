using System;
using System.IO;
using System.Text;
using System.Xml;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    internal sealed class TppXmlResponseBuilder
    {
        private readonly string _rootElementName;

        internal TppXmlResponseBuilder(string rootElementName)
        {
            _rootElementName = rootElementName;
        }

        internal string BuildXml(Action<XmlWriter> write)
        {
            using (var xml = new MemoryStream())
            using (var writer = XmlWriter.Create(xml, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
            {
                writer.WriteStartElement(_rootElementName);
                write(writer);
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();

                return Encoding.UTF8.GetString(xml.ToArray());
            }
        }
    }
}