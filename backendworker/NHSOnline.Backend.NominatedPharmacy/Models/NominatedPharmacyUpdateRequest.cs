using System;
using System.Globalization;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public class NominatedPharmacyUpdateRequest
    {
        private const string BoundaryEnd = "----=_MIME-Boundary--";

        private readonly Guid _messageId;
        private readonly string _hl7Time;
        private readonly bool _hasExistingNominatedPharmacy;
        private readonly string _updatedOdsCode;
        private readonly string _pertinentSerialChangeNumber;
        private readonly string _nhsNumber;
        private readonly INominatedPharmacyConfigurationSettings _config;


        public NominatedPharmacyUpdateRequest(
            string nhsNumber,
            bool hasExistingNominatedPharmacy,
            string updatedOdsCode,
            string pertinentSerialChangeNumber,
            INominatedPharmacyConfigurationSettings config)
        {
            _messageId = Guid.NewGuid();
            _hl7Time = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            _hasExistingNominatedPharmacy = hasExistingNominatedPharmacy;
            _updatedOdsCode = updatedOdsCode;
            _pertinentSerialChangeNumber = pertinentSerialChangeNumber;
            _nhsNumber = nhsNumber;
            _config = config;
        }

        public string Body()
        {
            return GetEbXmlBoundary() +
                   GetEbXmlBody() +
                   GetHl7Boundary() +
                   GetHl7Body() +
                   BoundaryEnd;
        }

        private string GetUpdateMode()
        {
            const string UpdateModeAdded = "added";
            const string UpdateModeAltered = "altered";

            return _hasExistingNominatedPharmacy ? UpdateModeAltered : UpdateModeAdded;
        }
        
        private static string GetEbXmlBoundary()
        {
            return "----=_MIME-Boundary\n" +
                   "Content-Id: <ebXMLHeader@spine.nhs.uk>\n" +
                   "Content-Type: text/xml; charset=UTF-8\n" +
                   "Content-Transfer-Encoding: 8bit\n\n";
        }

        private string GetEbXmlBody()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                   "<SOAP:Envelope xmlns:xsi=\"http://www.w3c.org/2001/XML-Schema-Instance\" xmlns:SOAP=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:eb=\"http://www.oasis-open.org/committees/ebxml-msg/schema/msg-header-2_0.xsd\" xmlns:hl7ebxml=\"urn:hl7-org:transport/ebxml/DSTUv1.0\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">" +
                   "<SOAP:Header>" +
                   "<eb:MessageHeader SOAP:mustUnderstand=\"1\" eb:version=\"2.0\">" +
                   "<eb:From>" +
                   "<eb:PartyId eb:type=\"urn:nhs:names:partyType:ocs+serviceInstance\">E87021-820463</eb:PartyId>" +
                   "</eb:From>" +
                   "<eb:To>" +
                   "<eb:PartyId eb:type=\"urn:nhs:names:partyType:ocs+serviceInstance\">YEA-801248</eb:PartyId>" +
                   "</eb:To>" +
                   "<eb:CPAId>S2011405A2055612</eb:CPAId>" +
                   $"<eb:ConversationId>{_messageId}</eb:ConversationId>" +
                   "<eb:Service>urn:nhs:names:services:pds</eb:Service>" +
                   "<eb:Action>PRPA_IN000203UK06</eb:Action>" +
                   "<eb:MessageData>" +
                   $"<eb:MessageId>{_messageId}</eb:MessageId>" +
                   $"<eb:Timestamp>{_hl7Time}</eb:Timestamp>" +
                   "</eb:MessageData>" +
                   "<eb:DuplicateElimination/>" +
                   "</eb:MessageHeader>" +
                   "<eb:AckRequested SOAP:mustUnderstand=\"1\" eb:version=\"2.0\" eb:signed=\"false\" SOAP:actor=\"urn:oasis:names:tc:ebxml-msg:actor:toPartyMSH\"/>" +
                   "<eb:SyncReply SOAP:mustUnderstand=\"1\" eb:version=\"2.0\" SOAP:actor=\"http://schemas.xmlsoap.org/soap/actor/next\"/>" +
                   "</SOAP:Header>" +
                   "<SOAP:Body>" +
                   "<eb:Manifest SOAP:mustUnderstand=\"1\" eb:version=\"2.0\">" +
                   $"<eb:Reference xlink:href=\"cid:{_messageId}@spine.nhs.uk\">" +
                   "<eb:Schema eb:location=\"http://www.nhsia.nhs.uk/schemas/HL7-Message.xsd\" eb:version=\"1.0\"/>" +
                   "<eb:Description xml:lang=\"en\">HL7 payload</eb:Description>" +
                   "<hl7ebxml:Payload style=\"HL7\" encoding=\"XML\" version=\"3.0\"/>" +
                   "</eb:Reference>" +
                   "</eb:Manifest>" +
                   "</SOAP:Body>" +
                   "</SOAP:Envelope>\n\n";
        }

        private string GetHl7Boundary()
        {
            return "----=_MIME-Boundary\n" +
                   $"Content-Id: <{_messageId}@spine.nhs.uk>\n" +
                   "Content-Type: application/xml; charset=UTF-8\n" +
                   "Content-Transfer-Encoding: 8bit\n\n";
        }

        private string GetHl7Body()
        {
            return "<PRPA_IN000203UK06 xmlns=\"urn:hl7-org:v3\">" +
                   $"<id root=\"{_messageId}\"/>" +
                   $"<creationTime value=\"{_hl7Time}\"/>" +
                   "<versionCode code=\"V3NPfIT3.0\"/>" +
                   "<interactionId root=\"2.16.840.1.113883.2.1.3.2.4.12\" extension=\"PRPA_IN000203UK06\"/>" +
                   "<processingCode code=\"P\"/>" +
                   "<processingModeCode code=\"T\"/>" +
                   "<acceptAckCode code=\"NE\"/>" +
                   "<communicationFunctionRcv>" +
                   "<device classCode=\"DEV\" determinerCode=\"INSTANCE\">" +
                   $"<id extension=\"{_config.SpineAccreditedSystemIdTo}\" root=\"1.2.826.0.1285.0.2.0.107\"/>" +
                   "</device>" +
                   "</communicationFunctionRcv>" +
                   "<communicationFunctionSnd>" +
                   "<device classCode=\"DEV\" determinerCode=\"INSTANCE\">" +
                   $"<id extension=\"{_config.SpineAccreditedSystemIdFrom}\" root=\"1.2.826.0.1285.0.2.0.107\"/>" +
                   "</device>" +
                   "</communicationFunctionSnd>" +
                   "<ControlActEvent xmlns=\"urn:hl7-org:v3\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
                   "xsi:schemaLocation=\"urn:hl7-org:v3 SpineXSD\\MCAI_MT040101UK03.xsd\" classCode=\"CACT\" moodCode=\"EVN\">" +
                   "<code code=\"1\" codeSystem=\"2.16.840.1.113883.2.1.3.2.4.17.40\" />" +
                   "<author typeCode=\"AUT\">" +
                   "<AgentPersonSDS classCode=\"AGNT\">" +
                   $"<id extension=\"{_config.PersonSdsRoleId}\" root=\"1.2.826.0.1285.0.2.0.67\"/>" +
                   "<agentPersonSDS classCode=\"PSN\" determinerCode=\"INSTANCE\">" +
                   $"<id extension=\"{_config.SdsUserId}\" root=\"1.2.826.0.1285.0.2.0.65\"/>" +
                   "</agentPersonSDS>" +
                   "<part typeCode=\"PART\">" +
                   "<partSDSRole classCode=\"ROL\">" +
                   $"<id extension=\"{_config.PartSdsRoleId}\" root=\"1.2.826.0.1285.0.2.1.104\"/>" +
                   "</partSDSRole>" +
                   "</part>" +
                   "</AgentPersonSDS>" +
                   "</author>" +
                   "<author1 typeCode=\"AUT\">" +
                   "<AgentSystemSDS classCode=\"AGNT\">" +
                   "<agentSystemSDS classCode=\"DEV\" determinerCode=\"INSTANCE\">" +
                   "<id extension=\"200000000355\" root=\"1.2.826.0.1285.0.2.0.107\"/>" +
                   "</agentSystemSDS>" +
                   "</AgentSystemSDS>" +
                   "</author1>" +
                   "<subject>" +
                   "<PdsUpdateRequest classCode=\"REG\" moodCode=\"RQO\">" +
                   "<code code=\"1\" codeSystem=\"2.16.840.1.113883.2.1.3.2.4.17.40\" />" +
                   "<pertinentInformation typeCode=\"PERT\">" +
                   "<pertinentSerialChangeNumber classCode=\"OBS\" moodCode=\"EVN\">" +
                   "<code code=\"2\" codeSystem=\"2.16.840.1.113883.2.1.3.2.4.17.35\"/>" +
                   $"<value value=\"{_pertinentSerialChangeNumber}\"/>" +
                   "</pertinentSerialChangeNumber>" +
                   "</pertinentInformation>" +
                   "<subject typeCode=\"SBJ\">" +
                   "<patientRole classCode=\"PAT\">" +
                   $"<id root=\"2.16.840.1.113883.2.1.4.1\" extension=\"{ _nhsNumber.RemoveWhiteSpace() }\"/>" +
                   "<patientPerson classCode=\"PSN\" determinerCode=\"INSTANCE\" >" +
                   "<playedOtherProviderPatient classCode=\"PAT\">" +
                   "<subjectOf typeCode=\"SBJ\">" +
                   $"<patientCareProvision classCode=\"PCPR\" moodCode=\"EVN\" updateMode=\"{ GetUpdateMode() }\">" +
                   "<code code=\"P1\" codeSystem=\"2.16.840.1.113883.2.1.3.2.4.17.37\"/>" +
                   "<id root=\"2.16.840.1.113883.2.1.3.2.4.18.1\" extension=\"P000000042\" />" +
                   "<performer typeCode=\"PRF\">" +
                   "<assignedOrganization classCode=\"ASSIGNED\">" +
                   $"<id root=\"2.16.840.1.113883.2.1.4.3\" extension=\"{_updatedOdsCode}\"/>" +
                   "</assignedOrganization>" +
                   "</performer>" +
                   "</patientCareProvision>" +
                   "</subjectOf>" +
                   "</playedOtherProviderPatient>" +
                   "</patientPerson>" +
                   "</patientRole>" +
                   "</subject>" +
                   "</PdsUpdateRequest>" +
                   "</subject>" +
                   "</ControlActEvent>" +
                   "</PRPA_IN000203UK06>\n";
        }
    }
}
