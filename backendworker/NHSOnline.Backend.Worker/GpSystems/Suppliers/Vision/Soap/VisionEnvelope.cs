using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Soap
{
    public class VisionEnvelope
    {
        public VisionEnvelope()
        {
        }

        public VisionEnvelope(X509Certificate2 certificate, string requestUsername)
        {
            GenerateGuids(out var binarySecurityTokenId, out var timeStampGuid, out var usernameGuid);

            XmlDocument xmlDocument = BuildCoreXml(certificate, requestUsername, usernameGuid, binarySecurityTokenId,
                timeStampGuid);

            ///////////////////////////////////////////////////////////////////
            // Now sign the timestamp - see https://stackoverflow.com/a/6467877
            ///////////////////////////////////////////////////////////////////
            var signedXml = new SignedXmlWithId(xmlDocument);

            // Set the signature properties
            var key = certificate.PrivateKey;
            signedXml.SigningKey = key;
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

            // Set the signature key info
            var keyInfo = new KeyInfo();
            XmlElement securityTokenReference =
                xmlDocument.CreateElement("wsse", "SecurityTokenReference", VisionNamespaces.Wsse);
            XmlElement subReference = xmlDocument.CreateElement("wsse", "Reference", VisionNamespaces.Wsse);
            subReference.SetAttribute("URI", "#" + binarySecurityTokenId);
            subReference.SetAttribute("ValueType", VisionNamespaces.WssX509Token);
            securityTokenReference.AppendChild(subReference);
            var keyInfoData = new KeyInfoNode(securityTokenReference);
            keyInfo.AddClause(keyInfoData);
            signedXml.KeyInfo = keyInfo;

            // sign the timestamp fragment
            var tsReference = new Reference { Uri = "#" + timeStampGuid };
            var transform = new XmlDsigExcC14NTransform();
            tsReference.AddTransform(transform);
            tsReference.DigestMethod = SignedXml.XmlDsigSHA1Url;
            signedXml.AddReference(tsReference);

            // Now compute the signature
            signedXml.ComputeSignature();

            // Add the signature to the the main envelope
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("wsse", VisionNamespaces.Wsse);
            nsmgr.AddNamespace("wsu", VisionNamespaces.Wsu);
            nsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            // Insert the digital signature after the Binary Security Token
            var securityNode = xmlDocument.SelectSingleNode("/soapenv:Envelope/soapenv:Header/wsse:Security", nsmgr);
            var tokenNode =
                xmlDocument.SelectSingleNode("/soapenv:Envelope/soapenv:Header/wsse:Security/wsse:BinarySecurityToken",
                    nsmgr);
            securityNode.InsertAfter(xmlDigitalSignature, tokenNode);
            Envelope = xmlDocument;
        }

        private static XmlDocument BuildCoreXml(X509Certificate2 certificate, string requestUsername,
            string usernameGuid,
            string binarySecurityTokenId, string timeStampGuid)
        {
            XmlDocument xmlDocument;

            using (var stream = new MemoryStream())
            {
                Encoding utf8 = new UTF8Encoding(false); // omit BOM
                using (var writer = new XmlTextWriter(stream, utf8))
                {
                    // Need to create a timestamp with a 5 minute offset
                    var dt = DateTime.UtcNow;
                    var timeNow = dt.ToString("o").Substring(0, 23) + "Z";
                    var timePlus15 = dt.AddMinutes(15).ToString("o").Substring(0, 23) + "Z";

                    // Start Envelope
                    writer.WriteStartDocument();
                    writer.WriteStartElement("soapenv", "Envelope", VisionNamespaces.SoapEnv);
                    writer.WriteAttributeString("xmlns", "urn", null, "urn:vision");

                    // Start Header
                    writer.WriteStartElement("soapenv", "Header", null);

                    writer.WriteAttributeString("xmlns", "wsa", null, VisionNamespaces.Wsa);
                    writer.WriteStartElement("wsse", "Security", VisionNamespaces.Wsse);
                    writer.WriteAttributeString("xmlns", "wsu", null, VisionNamespaces.Wsu);

                    // Start Username Token
                    // Username token needs the username element only
                    writer.WriteStartElement("wsse", "UsernameToken", null);
                    writer.WriteAttributeString("wsu", "Id", null, usernameGuid);
                    writer.WriteElementString("wsse", "Username", null, requestUsername);
                    writer.WriteEndElement();
                    // End Username Token

                    // Start Binary Security Token
                    writer.WriteStartElement("wsse", "BinarySecurityToken", null);
                    writer.WriteAttributeString("EncodingType", VisionNamespaces.WssBase64Binary);

                    // NOTE: Value type in SOAP UI was originally X509PKIPathv1. Have to use X509v3 as it is not
                    // possible to generate X509PKIPathv1 here
                    writer.WriteAttributeString("ValueType", VisionNamespaces.WssX509Token);
                    writer.WriteAttributeString("wsu", "Id", null, binarySecurityTokenId);

                    byte[] rawData = certificate.GetRawCertData();

                    writer.WriteBase64(rawData, 0, rawData.Length);

                    writer.WriteEndElement();
                    // Finished BinarySecurityToken

                    // Start TimeStamp
                    writer.WriteStartElement("wsu", "Timestamp", null);
                    writer.WriteAttributeString("wsu", "Id", null, timeStampGuid);
                    writer.WriteElementString("wsu", "Created", null, timeNow);
                    writer.WriteElementString("wsu", "Expires", null, timePlus15);
                    writer.WriteEndElement();
                    // Finished Timestamp

                    writer.WriteEndElement();
                    // End security

                    writer.WriteElementString("wsa", "Action", null, "Vision");
                    writer.WriteElementString("wsa", "MessageID", null,
                        String.Format("uuid:{0}", Guid.NewGuid().ToString()));

                    writer.WriteEndElement();
                    // End Header

                    // Begin Body
                    writer.WriteStartElement("soapenv", "Body", VisionNamespaces.SoapEnv);
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

        private static void GenerateGuids(out string binarySecurityTokenId, out string timeStampGuid,
            out string usernameGuid)
        {
            binarySecurityTokenId = string.Format("X509-{0}", Guid.NewGuid().ToString("N"));
            timeStampGuid = string.Format("TS-{0}", Guid.NewGuid().ToString("N"));
            usernameGuid = string.Format("UsernameToken-{0}", Guid.NewGuid().ToString("N"));
        }

        public XmlDocument Envelope { get; set; }

        public void AddBody(object body)
        {
            XmlNode objectXml = SerializeToXmlDocument(body).DocumentElement;

            XmlNode newObjectXml = Envelope.ImportNode(objectXml, true);

            var nsmgr = new XmlNamespaceManager(Envelope.NameTable);
            nsmgr.AddNamespace("wsse", VisionNamespaces.Wsse);
            nsmgr.AddNamespace("wsu", VisionNamespaces.Wsu);
            nsmgr.AddNamespace("soapenv", VisionNamespaces.SoapEnv);
            var bodyNode = Envelope.SelectSingleNode("/soapenv:Envelope//soapenv:Body", nsmgr);
            bodyNode.AppendChild(newObjectXml);
        }

        public XmlDocument SerializeToXmlDocument(object input)
        {
            XmlSerializer ser = new XmlSerializer(input.GetType());
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("vision", "urn:vision");

            XmlDocument xd = null;

            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, input, ns);

                memStm.Position = 0;

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;

                using (var xtr = XmlReader.Create(memStm, settings))
                {
                    xd = new XmlDocument();
                    xd.Load(xtr);
                }
            }

            return xd;
        }

        private class SignedXmlWithId : SignedXml
        {
            public SignedXmlWithId(XmlDocument xml) : base(xml)
            {
            }

            public SignedXmlWithId(XmlElement xmlElement)
                : base(xmlElement)
            {
            }

            public override XmlElement GetIdElement(XmlDocument doc, string id)
            {
                // Check to see if it's a standard ID reference
                var idElem = base.GetIdElement(doc, id);

                if (idElem != null) return idElem;

                var nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("wsu", VisionNamespaces.Wsu);

                idElem = doc.SelectSingleNode("//*[@wsu:Id=\"" + id + "\"]", nsManager) as XmlElement;

                return idElem;
            }
        }
    }
}