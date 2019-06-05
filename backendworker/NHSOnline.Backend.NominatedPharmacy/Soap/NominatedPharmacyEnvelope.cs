using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions;

namespace NHSOnline.Backend.NominatedPharmacy.Soap
{
    public class NominatedPharmacyEnvelope
    {
        private const string SoapEnv = "SOAP-ENV";

        private readonly INominatedPharmacyConfigurationSettings _config;

        public XmlDocument Envelope { get; set; }

        public NominatedPharmacyEnvelope(IServiceDefinition serviceDefinition, INominatedPharmacyConfigurationSettings config)
        {
            XmlDocument xmlDocument = BuildCoreXml(serviceDefinition, config);
            
            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");

            _config = config;
            Envelope = xmlDocument;
        }

        private static XmlDocument BuildCoreXml(IServiceDefinition serviceDefinition, INominatedPharmacyConfigurationSettings config)
        {
            const string Xmlns = "xmlns";
            const string Wsa = "wsa";
            const string hl7 = "hl7";
            var messageId = Guid.NewGuid();
            XmlDocument xmlDocument;

            using (var stream = new MemoryStream())
            {
                Encoding utf8 = new UTF8Encoding(false); // omit BOM
                using (var writer = new XmlTextWriter(stream, utf8))
                {
                    // Start Envelope
                    writer.WriteStartDocument();
                    writer.WriteStartElement(SoapEnv, "Envelope", NominatedPharmacyNamespaces.SoapEnv);
                    writer.WriteAttributeString(Xmlns, Wsa, null, NominatedPharmacyNamespaces.Addressing);
                    writer.WriteAttributeString(Xmlns, hl7, null, NominatedPharmacyNamespaces.Hl7);

                    // Start Header
                    writer.WriteStartElement(SoapEnv, "Header", null);

                    writer.WriteElementString(Wsa, "MessageID", null, String.Format(CultureInfo.InvariantCulture, "uuid:{0}", messageId.ToString()));
                    writer.WriteElementString(Wsa, "Action", null, serviceDefinition.SoapActionName);
                    writer.WriteElementString(Wsa, "To", null, config.PdsQueryTo);
                    writer.WriteStartElement(Wsa, "From", null);
                    writer.WriteElementString(Wsa, "Address", null, config.PdsQueryFromAddress);
                    writer.WriteEndElement();
                    
                    writer.WriteStartElement(hl7, "communicationFunctionRcv", null);
                    writer.WriteStartElement(hl7, "device", null);
                    writer.WriteStartElement(hl7, "id", null);
                    writer.WriteAttributeString("root", "1.2.826.0.1285.0.2.0.107");
                    writer.WriteAttributeString("extension", config.SpineAccreditedSystemIdTo);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    
                    writer.WriteStartElement(hl7, "communicationFunctionSnd", null);
                    writer.WriteStartElement(hl7, "device", null);
                    writer.WriteStartElement(hl7, "id", null);
                    writer.WriteAttributeString("root", "1.2.826.0.1285.0.2.0.107");
                    writer.WriteAttributeString("extension", config.SpineAccreditedSystemIdFrom);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    writer.WriteStartElement(Wsa, "ReplyTo", null);
                    writer.WriteElementString(Wsa, "Address", null, config.PdsQueryFromAddress);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    // End Header

                    // Begin Body
                    writer.WriteStartElement(SoapEnv, "Body", NominatedPharmacyNamespaces.SoapEnv);
                    writer.WriteEndElement();
                    // End Body

                    writer.WriteEndElement();
                    // End Envelope
                }

                // Convert the stream into an XML Document for modification
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(Encoding.UTF8.GetString(stream.ToArray()));
            }

            return xmlDocument;
        }

        public void AddBody(object body)
        {
            XmlNode objectXml = SerializeToXmlDocument(body).DocumentElement;

            XmlNode newObjectXml = Envelope.ImportNode(objectXml, true);

            var nsmgr = new XmlNamespaceManager(Envelope.NameTable);
            nsmgr.AddNamespace(SoapEnv, NominatedPharmacyNamespaces.SoapEnv);
            var bodyNode = Envelope.SelectSingleNode("/SOAP-ENV:Envelope//SOAP-ENV:Body", nsmgr);
            bodyNode.AppendChild(newObjectXml);
        }

        public static XmlDocument SerializeToXmlDocument(object input)
        {
            XmlSerializer ser = new XmlSerializer(input.GetType());
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            XmlDocument xd;

            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, input, ns);

                memStm.Position = 0;

                XmlReaderSettings settings = new XmlReaderSettings
                {
                    IgnoreWhitespace = true
                };

                using (var xtr = XmlReader.Create(memStm, settings))
                {
                    xd = new XmlDocument();
                    xd.Load(xtr);
                }
            }

            return xd;
        }
    }
}