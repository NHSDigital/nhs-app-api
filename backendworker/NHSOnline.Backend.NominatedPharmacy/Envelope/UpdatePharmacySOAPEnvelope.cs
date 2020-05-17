using System.IO;
using System.Text;
using System.Xml;

namespace NHSOnline.Backend.NominatedPharmacy.Envelope
{
    public static class UpdatePharmacySOAPEnvelope
    {
         public static XmlDocument BuildSOAPEnvelope(string messageID, string hl7Time, string partyIdFrom, string partyIdTo, string cpaId)
        {
            const string SOAP = "SOAP";
            const string Xmlns = "xmlns";
            const string hl7EbXML = "hl7ebxml";
            const string Eb = "eb";
            const string Xlink = "xlink";
            const string MustUnderstand = "mustUnderstand";
            const string Version = "version";

            XmlDocument xmlDocument;
            using (var stream = new MemoryStream())
            {
                Encoding utf8 = new UTF8Encoding(false);
                using (var writer = new XmlTextWriter(stream, utf8))
                {
                    // Start Envelope
                    writer.WriteStartDocument();
                    writer.WriteStartElement(SOAP, "Envelope","http://schemas.xmlsoap.org/soap/envelope/");
                    writer.WriteAttributeString(Xmlns, "xsi", null,"http://www.w3c.org/2001/XML-Schema-Instance");
                    writer.WriteAttributeString(Xmlns, Eb, null,"http://www.oasis-open.org/committees/ebxml-msg/schema/msg-header-2_0.xsd");
                    writer.WriteAttributeString(Xmlns, hl7EbXML, null,"urn:hl7-org:transport/ebxml/DSTUv1.0");
                    writer.WriteAttributeString(Xmlns, Xlink, null,"http://www.w3.org/1999/xlink");

                   // Start Header
                    writer.WriteStartElement(SOAP, "Header", null);

                    // Start MessageHeader
                    writer.WriteStartElement(Eb, "MessageHeader", null);
                    writer.WriteAttributeString(SOAP, MustUnderstand, null, "1");
                    writer.WriteAttributeString(Eb, Version, null,"2.0");

                    // Start From
                    writer.WriteStartElement(Eb, "From", null);

                    // Start PartyId
                    writer.WriteStartElement(Eb, "PartyId", null);
                    writer.WriteAttributeString(Eb, "type",null,"urn:nhs:names:partyType:ocs+serviceInstance");
                    writer.WriteRaw( partyIdFrom );
                    writer.WriteEndElement();
                    // End PartyId

                    writer.WriteEndElement();
                    // End From

                    // Start To
                    writer.WriteStartElement(Eb, "To", null);

                    // Start PartyId
                    writer.WriteStartElement(Eb, "PartyId", null);
                    writer.WriteAttributeString(Eb, "type", null,"urn:nhs:names:partyType:ocs+serviceInstance");
                    writer.WriteRaw( partyIdTo );
                    writer.WriteEndElement();
                    // End PartyId

                    writer.WriteEndElement();
                    // End To

                    // Start CPAId
                    writer.WriteStartElement(Eb, "CPAId", null);
                    writer.WriteRaw(cpaId);
                    writer.WriteEndElement();
                    // End CPAId

                    // Start ConversationId
                    writer.WriteStartElement(Eb, "ConversationId", null);
                    writer.WriteRaw(messageID);
                    writer.WriteEndElement();
                    // End ConversationId

                    // Start Service
                    writer.WriteStartElement(Eb, "Service", null);
                    writer.WriteRaw("urn:nhs:names:services:pds");
                    writer.WriteEndElement();
                    // End Service

                    // Start Action
                    writer.WriteStartElement(Eb, "Action", null);
                    writer.WriteRaw("PRPA_IN000203UK03");
                    writer.WriteEndElement();
                    // End Action

                    // Start MessageData
                    writer.WriteStartElement(Eb, "MessageData", null);

                    // Start MessageId
                    writer.WriteStartElement(Eb, "MessageId", null);
                    writer.WriteRaw(messageID);
                    writer.WriteEndElement();
                    // End MessageId

                    // Start Timestamp
                    writer.WriteStartElement(Eb, "Timestamp", null);
                    writer.WriteRaw(hl7Time);
                    writer.WriteEndElement();
                    // End Timestamp

                    writer.WriteEndElement();
                    // End MessageData

                    // Start DuplicateElimination
                    writer.WriteStartElement(Eb, "DuplicateElimination", null);
                    writer.WriteEndElement();
                    // End DuplicateElimination

                    writer.WriteEndElement();
                    // End MessageHeader

                    // Start AckRequested
                    writer.WriteStartElement(Eb, "AckRequested", null);
                    writer.WriteAttributeString(SOAP, MustUnderstand, null, "1");
                    writer.WriteAttributeString(Eb, Version, null, "2.0");
                    writer.WriteAttributeString(Eb, "signed", null, "false");
                    writer.WriteAttributeString(SOAP, "actor", null,"urn:oasis:names:tc:ebxml-msg:actor:toPartyMSH");
                    writer.WriteEndElement();
                    // End AckRequested

                    // Start AckRequested
                    writer.WriteStartElement(Eb, "SyncReply", null);
                    writer.WriteAttributeString(SOAP, MustUnderstand, null, "1");
                    writer.WriteAttributeString(Eb, Version, null, "2.0");
                    writer.WriteAttributeString(SOAP, "actor", null,"http://schemas.xmlsoap.org/soap/actor/next");
                    writer.WriteEndElement();
                    // End AckRequested

                    writer.WriteEndElement();
                    // End Header


                   // Begin Body
                    writer.WriteStartElement(SOAP, "Body", null);

                    // Start Manifest
                    writer.WriteStartElement(Eb, "Manifest", null);
                    writer.WriteAttributeString(SOAP, MustUnderstand, null, "1");
                    writer.WriteAttributeString(Eb, Version, null, "2.0");

                    // Start Reference
                    writer.WriteStartElement(Eb, "Reference", null);
                    writer.WriteAttributeString(Xlink, "href", null,"cid:{" + messageID + "}@spine.nhs.uk");

                    // Start Schema
                    writer.WriteStartElement(Eb, "Schema", null);
                    writer.WriteAttributeString(Eb, "location", null,"http://www.nhsia.nhs.uk/schemas/HL7-Message.xsd");
                    writer.WriteAttributeString(Eb, Version, null, "1.0");
                    writer.WriteEndElement();
                    // End Schema

                    // Start Description
                    writer.WriteStartElement(Eb, "Description", null);
                    writer.WriteAttributeString("xml", "lang", null,"en");
                    writer.WriteRaw("HL7 payload");
                    writer.WriteEndElement();
                    // End Description

                    // Start hl7ebxml
                    writer.WriteStartElement(hl7EbXML, "Payload", null);
                    writer.WriteAttributeString("style", "HL7");
                    writer.WriteAttributeString("encoding","XML");
                    writer.WriteAttributeString(Version,"3.0");
                    writer.WriteEndElement();
                    // End hl7ebxml

                    writer.WriteEndElement();
                    // End Reference

                    writer.WriteEndElement();
                    // End Manifest

                    writer.WriteEndElement();
                    // End Body

                    writer.WriteEndElement();
                    // End Envelope
                }
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(Encoding.UTF8.GetString(stream.ToArray()));
            }
            return xmlDocument;
        }
    }
}