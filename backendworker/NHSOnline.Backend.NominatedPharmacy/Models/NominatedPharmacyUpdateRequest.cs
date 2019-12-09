using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions;
using NHSOnline.Backend.NominatedPharmacy.Soap;
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
                   GetEbXmlBody() + "\n" +
                   GetHl7Boundary() + "\n" +
                   GetHl7Body() + "\n" +
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
            var envelope = UpdatePharmacySOAPEnvelope.BuildSOAPEnvelope(
                _messageId.ToString(),
                _hl7Time,
                _config.PdsUpdateConfigurationSettings.FromPartyId,
                _config.PdsUpdateConfigurationSettings.ToPartyId,
                _config.PdsUpdateConfigurationSettings.CpaId);

            return envelope.OuterXml;
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
            var controlActEventBody = new UpdateNominatedPharmacyTypes.ControlActEvent
            {
                ClassCode = "CACT",
                MoodCode = "EVN",
                Xsi = "http://www.w3.org/2001/XMLSchema-instance",
                SchemaLocation = "urn:hl7-org:v3 SpineXSD\\MCAI_MT040101UK03.xsd",
                Code = new NominatedPharmacyTypes.CodeElement
                {
                    Code = "1",
                    CodeSystem = "2.16.840.1.113883.2.1.3.2.4.17.40"
                },
                Author1 = new NominatedPharmacyTypes.Author1
                {
                    TypeCode = "AUT",
                    AgentSystemSDS = new NominatedPharmacyTypes.AgentSystemSDS()
                    {
                        ClassCode = "AGNT",
                        AgentSystemSDSInner = new NominatedPharmacyTypes.AgentSystemSDSInner
                        {
                            ClassCode = "DEV",
                            DeterminerCode = "INSTANCE",
                            Id = new NominatedPharmacyTypes.Id
                            {
                                Root = "1.2.826.0.1285.0.2.0.107",
                                Extension = _config.PdsUpdateConfigurationSettings.FromAsid,
                            }
                        }
                    }
                },
                SubjectOuter = new UpdateNominatedPharmacyTypes.SubjectOuter
                {
                    PdsUpdateRequest = new UpdateNominatedPharmacyTypes.PdsUpdateRequest
                    {
                        ClassCode = "REG",
                        MoodCode = "RQO",
                        Code = new NominatedPharmacyTypes.CodeElement
                        {
                            Code = "1",
                            CodeSystem = "2.16.840.1.113883.2.1.3.2.4.17.40"
                        },
                        PertinentInformation = new UpdateNominatedPharmacyTypes.PertinentInformation
                        {
                            TypeCode = "PERT",
                            PertinentSerialChangeNumber =
                                new UpdateNominatedPharmacyTypes.PertinentSerialChangeNumber
                                {
                                    ClassCode = "OBS",
                                    MoodCode = "EVN",
                                    Code = new NominatedPharmacyTypes.CodeElement
                                    {
                                        Code = "2",
                                        CodeSystem = "2.16.840.1.113883.2.1.3.2.4.17.35"
                                    },
                                    Value = new UpdateNominatedPharmacyTypes.ValueElement
                                    {
                                        value = _pertinentSerialChangeNumber
                                    }
                                }
                        },
                        Subject = new UpdateNominatedPharmacyTypes.Subject
                        {
                            TypeCode = "SBJ",
                            PatientRole = new UpdateNominatedPharmacyTypes.PatientRole
                            {
                                ClassCode = "PAT",
                                Id = new NominatedPharmacyTypes.Id
                                {
                                    Root = "2.16.840.1.113883.2.1.4.1",
                                    Extension = _nhsNumber.RemoveWhiteSpace()
                                },
                                PatientPerson = new UpdateNominatedPharmacyTypes.PatientPerson
                                {
                                    ClassCode = "PSN",
                                    DeterminerCode = "INSTANCE",
                                    PlayedOtherProviderPatient =
                                        new UpdateNominatedPharmacyTypes.PlayedOtherProviderPatient
                                        {
                                            ClassCode = "PAT",
                                            SubjectOf = new UpdateNominatedPharmacyTypes.SubjectOf
                                            {
                                                TypeCode = "SBJ",
                                                PatientCareProvision =
                                                    new UpdateNominatedPharmacyTypes.PatientCareProvision
                                                    {
                                                        ClassCode = "PCPR",
                                                        MoodCode = "EVN",
                                                        UpdateMode = GetUpdateMode(),
                                                        Code = new NominatedPharmacyTypes.CodeElement
                                                        {
                                                            Code = "P1",
                                                            CodeSystem = "2.16.840.1.113883.2.1.3.2.4.17.37"
                                                        },
                                                        Id = new NominatedPharmacyTypes.Id
                                                        {
                                                            Root = "2.16.840.1.113883.2.1.3.2.4.18.1",
                                                            Extension = "P000000042"
                                                        },
                                                        Performer = new UpdateNominatedPharmacyTypes.Performer
                                                        {
                                                            TypeCode = "PRF",
                                                            AssignedOrganization =
                                                                new UpdateNominatedPharmacyTypes.
                                                                    AssignedOrganization
                                                                    {
                                                                        ClassCode = "ASSIGNED",
                                                                        Id = new NominatedPharmacyTypes.Id
                                                                        {
                                                                            Root = "2.16.840.1.113883.2.1.4.3",
                                                                            Extension = _updatedOdsCode
                                                                        }
                                                                    }
                                                        }
                                                    }
                                            }
                                        }
                                }
                            }
                        }
                    }
                }
            };

            XmlDocument controlActDoc = new XmlDocument();
            controlActDoc.LoadXml(controlActEventBody.SerializeXml());
            controlActDoc.DocumentElement.SetAttribute("xmlns", "urn:hl7-org:v3");

            var requestBody = new UpdateNominatedPharmacyTypes.PRPAIN000203UK03
            {
                Id = new NominatedPharmacyTypes.Id
                {
                    Root = _messageId.ToString()
                },
                CreationTime = new NominatedPharmacyTypes.CreationTime
                {
                    Value = _hl7Time
                },
                VersionCode = new NominatedPharmacyTypes.VersionCode
                {
                    Code = "V3NPfIT3.0"
                },
                InteractionId = new NominatedPharmacyTypes.InteractionId
                {
                    Root = "2.16.840.1.113883.2.1.3.2.4.12",
                    Extension = "PRPA_IN000203UK03"
                },
                ProcessingCode = new NominatedPharmacyTypes.ProcessingCode
                {
                    Code = "P"
                },
                ProcessingModeCode = new NominatedPharmacyTypes.ProcessingModeCode
                {
                    Code = "T"
                },
                AcceptAckCode = new NominatedPharmacyTypes.AcceptAckCode
                {
                    Code = "NE"
                },
                CommunicationFunctionRcv = CreateCommunicationFunctionRcv(_config.PdsUpdateConfigurationSettings.ToAsid),
                CommunicationFunctionSnd = CreateCommunicationFunctionSnd(_config.PdsUpdateConfigurationSettings.FromAsid),
                ControlActEvent = controlActEventBody
            };

            var xdoc = RemoveXmlns(requestBody.SerializeXml()).DocumentElement;
            xdoc.SetAttribute("xmlns", "urn:hl7-org:v3");
            return xdoc.OuterXml;
        }

        private static XmlDocument RemoveXmlns(string xml)
        {
            var d = XDocument.Parse(xml);
            d.Root.Descendants().Attributes().Where(x => x.IsNamespaceDeclaration).Remove();

            foreach (var elem in d.Descendants())
                elem.Name = elem.Name.LocalName;

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(d.CreateReader());

            return xmlDocument;
        }

        public static NominatedPharmacyTypes.CommunicationFunctionSnd CreateCommunicationFunctionSnd(string extension)
        {
            return new NominatedPharmacyTypes.CommunicationFunctionSnd
            {
                Device = CreateDevice(extension)
            };
        }
        
        public static NominatedPharmacyTypes.CommunicationFunctionRcv CreateCommunicationFunctionRcv(string extension)
        {
            return new NominatedPharmacyTypes.CommunicationFunctionRcv
            {
                Device = CreateDevice(extension)
            };
        }

        private static NominatedPharmacyTypes.Device CreateDevice(string extension)
        {
            return new NominatedPharmacyTypes.Device
            {
                ClassCode = "DEV",
                DeterminerCode = "INSTANCE",
                Id = new NominatedPharmacyTypes.Id
                {
                    Extension = extension,
                    Root = "1.2.826.0.1285.0.2.0.107"
                }
            };
        }
    }
}
