using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Spine.Models;

namespace NHSOnline.HttpMocks.Spine
{

    [SuppressMessage("Usage", "CA1506: Avoid excessive class coupling",
        Justification = "Types required for mocking")]
    public class SpineDefaultBehaviour: ISpineBehaviour
    {
        private const string Ip = "10.0.225.22";
        private const string FromAsid = "200000000355";
        private const string ToAsid = "200000000354";

        public IActionResult Behave()
        {

            return new JsonResult(new Envelope
            {
                Crs = "http://national.carerecords.nhs.uk/schema/crs/",
                SOAPENV = "http://schemas.xmlsoap.org/soap/envelope/",
                Wsa = "http://schemas.xmlsoap.org/ws/2004/08/addressing",
                Hl7 = "urn:hl7-org:v3",
                Xmlns = "urn:hl7-org:v3",
                Header = new Header
                {
                    MessageId = "uuid:7C24CDCA-5C5C-11E9-B0FB-F4034348F02E",
                    Action = "urn:nhs:names:services:pdsquery/QUPA_IN000009UK03",
                    To = Ip,
                    From = new From
                    {
                        Address = "https://192.168.128.11/sync-service",
                    },
                    CommunicationFunctionRcv = new CommunicationFunctionRcv
                    {
                        TypeCode = "RCV",
                        Device = new Device
                        {
                            ClassCode = "DEV",
                            DeterminerCode = "INSTANCE",
                            Id = new Id
                            {
                                Root = "1.2.826.0.1285.0.2.0.107",
                                Extension = ToAsid,
                            }
                        },
                    },
                    CommunicationFunctionSnd = new CommunicationFunctionSnd
                    {
                        Device = new Device
                        {
                            ClassCode = "DEV",
                            DeterminerCode = "INSTANCE",
                            Id = new Id
                            {
                                Root = "1.2.826.0.1285.0.2.0.107",
                                Extension = FromAsid,
                            }
                        }
                    },
                    RelatesTo = "uuid:289852c1-b232-4ae2-b6c8-f3556e44e2b8",
                },
                Body = new Body
                {
                    RetrievalQueryResponse = new RetrievalQueryResponse
                    {
                        QUPAIN000009UK03 = new QUPAIN000009UK03
                        {
                            Id = new Id
                            {
                                Root = "7C24CDCA-5C5C-11E9-B0FB-F4034348F02E"
                            },
                            CreationTime = new CreationTime {Value = "20190411131950"},
                            VersionCode = new VersionCode {Code = "3NPfIT6.3.01"},
                            InteractionId = new InteractionId
                            {
                                Root = "2.16.840.1.113883.2.1.3.2.4.12",
                                Extension = "QUPA_IN000009UK03",
                            },
                            ProcessingCode = new ProcessingCode
                            {
                                Code = "P"
                            },
                            ProcessingModeCode = new ProcessingModeCode
                            {
                                Code = "T"
                            },
                            AcceptAckCode = new AcceptAckCode
                            {
                                Code = "NE"
                            },
                            Acknowledgement = new Acknowledgement
                            {
                                TypeCode = "AA",
                                MessageRef = new MessageRef
                                {
                                    Id = new Id
                                    {
                                        Root = "289852c1-b232-4ae2-b6c8-f3556e44e2b8"
                                    }
                                }
                            },
                            CommunicationFunctionRcv = new CommunicationFunctionRcv
                            {
                                TypeCode = "RCV",
                                Device = new Device
                                {
                                    ClassCode = "DEV",
                                    DeterminerCode = "INSTANCE",
                                    Id = new Id
                                    {
                                        Root = "1.2.826.0.1285.0.2.0.107",
                                        Extension = ToAsid
                                    }
                                },
                            },
                            CommunicationFunctionSnd = new CommunicationFunctionSnd
                            {
                                TypeCode = "SND",
                                Device = new Device
                                {
                                    ClassCode = "DEV",
                                    DeterminerCode = "INSTANCE",
                                    Id = new Id
                                    {
                                        Root = "1.2.826.0.1285.0.2.0.107",
                                        Extension = FromAsid
                                    }
                                }
                            },
                            ControlActEvent = new ControlActEvent
                            {
                                ClassCode = "CACT",
                                MoodCode = "EVN",
                                Author1 = new Author1
                                {
                                    TypeCode = "AUT",
                                    AgentSystemSDS = new AgentSystemSDS
                                    {
                                        ClassCode = "AGNT",
                                        agentSystemSDS = new AgentSystemSDS2
                                        {
                                            ClassCode = "DEV",
                                            DeterminerCode = "INSTANCE",
                                            Id = new Id
                                            {
                                                Root = "1.2.826.0.1285.0.2.0.107",
                                                Extension = "200000000355"
                                            }
                                        }
                                    }
                                },
                                Subject = new Subject
                                {
                                    TypeCode = "SUBJ",
                                    PdsResponse = new PDSResponse
                                    {
                                        Xsi = "http://www.w3.org/2001/XMLSchema-instance",
                                        ClassCode = "OBS",
                                        MoodCode = "EVN",
                                        PertinentInformation = new PertinentInformation
                                        {
                                            TypeCode = "PERT",
                                            PertinentSerialChangeNumber = new PertinentSerialChangeNumber
                                            {
                                                ClassCode = "OBS",
                                                MoodCode = "EVN",
                                                Code = new Code
                                                {
                                                    CodeSystem = "2.16.840.1.113883.2.1.3.2.4.17.35",
                                                    code = "2"
                                                },
                                                Value = new Value
                                                {
                                                    value = "2"
                                                },
                                            },
                                        },
                                        Subject = new Subject
                                        {
                                            TypeCode = "SBJ",
                                            PatientRole = new PatientRole
                                            {
                                                ClassCode = "PAT",
                                                Id = new Id
                                                {
                                                    Root = "2.16.840.1.113883.2.1.4.1",
                                                    Extension = "1001001010"
                                                },
                                                PatientPerson = new PatientPerson
                                                {
                                                    ClassCode = "PSN",
                                                    DeterminerCode = "INSTANCE",
                                                    AdministrativeGenderCode = new AdministrativeGenderCode
                                                    {
                                                        Code = "1"
                                                    },
                                                    BirthTime = new BirthTime
                                                    {
                                                        Value = "19800203"
                                                    },
                                                    PlayedOtherProviderPatient = new PlayedOtherProviderPatient
                                                    {
                                                        ClassCode = "PAT",
                                                        SubjectOf = new SubjectOf
                                                        {
                                                            PatientCareProvisionEvent = new PatientCareProvisionEvent
                                                            {
                                                                ClassCode = "PCPR",
                                                                MoodCode = "EVN",
                                                                Code = new Code
                                                                {
                                                                    CodeSystem = "2.16.840.1.113883.2.1.3.2.4.17.37",
                                                                    code = "1"
                                                                },
                                                                EffectiveTime = new EffectiveTime
                                                                {
                                                                    Low = new Low
                                                                    {
                                                                        Value = "19630422",
                                                                    }
                                                                },
                                                                Id = new Id
                                                                {
                                                                    Root = "2.16.840.1.113883.2.1.3.2.4.18.1",
                                                                    Extension = "HFmjd"
                                                                },
                                                                Performer = new Performer
                                                                {
                                                                    TypeCode = "PRF",
                                                                    AssignedEntity = new AssignedEntity
                                                                    {
                                                                        ClassCode = "ASSIGNED",
                                                                        Id = new Id
                                                                        {
                                                                            Root = "2.16.840.1.113883.2.1.3.2.4.18.1",
                                                                            Extension = "E87021"
                                                                        },

                                                                    }
                                                                }
                                                            }
                                                        }
                                                    },
                                                    COCTMT000201UK02PartOfWhole = new COCTMT000201UK02PartOfWhole
                                                    {
                                                        Addr = new Addr
                                                        {
                                                            Use = "H",
                                                            Id = new Id
                                                            {
                                                                Root = "2.16.840.1.113883.2.1.3.2.4.18.1",
                                                                Extension = "EwEOT"
                                                            },
                                                            PostalCode = "NW8 6PA",
                                                            StreetAddressLine = new List<string?>
                                                            {
                                                                "SUFFOLK HOUSE",
                                                                "COCHRANE MEWS",
                                                                "",
                                                                "LONDON",
                                                                ""
                                                            },
                                                            UseablePeriod = new UseablePeriod
                                                            {
                                                                Low = new Low
                                                                {
                                                                    Value = "20120913"
                                                                }
                                                            }
                                                        }
                                                    },
                                                    COCTMT000203UK02PartOfWhole = new COCTMT000203UK02PartOfWhole
                                                    {
                                                        ClassCode = "PART",
                                                        PartPerson = new PartPerson
                                                        {
                                                            ClassCode = "PSN",
                                                            DeterminerCode = "INSTANCE",
                                                            Name = new Name
                                                            {
                                                                Id = new Id
                                                                {
                                                                    Root = "2.16.840.1.113883.2.1.3.2.4.18.1",
                                                                    Extension = "Jpggc",
                                                                },
                                                                Prefix = "MR",
                                                                Use = "L",
                                                                Family = "EMIS",
                                                                Given = new List<string?> {"Roland", "Lionel"},
                                                                ValidTime = new ValidTime
                                                                {
                                                                    Low = new Low
                                                                    {
                                                                        Value = "20090308",
                                                                    }
                                                                }
                                                            }
                                                        },
                                                    }
                                                },
                                                SubjectOf8 = new SubjectOf8
                                                {
                                                    TypeCode = "SBJ",
                                                    PreviousNhsContact = new PreviousNhsContact
                                                    {
                                                        ClassCode = "OBS",
                                                        MoodCode = "EVN",
                                                        Code = new Code
                                                        {
                                                            code = "17",
                                                            CodeSystem = "2.16.840.1.113883.2.1.3.2.4.17.35"
                                                        },
                                                        Value = new Value
                                                        {
                                                            CodeSystem = "2.16.840.1.113883.2.1.3.2.4.17.38",
                                                            Code = "0"
                                                        }
                                                    }
                                                }
                                            },
                                        }
                                    },
                                },
                                QueryAck = new QueryAck
                                {
                                    QueryResponseCode = new QueryResponseCode {Code = "OK"}
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}