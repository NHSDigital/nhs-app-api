using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy.Models;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyService : INominatedPharmacyService
    {
        private readonly ILogger<NominatedPharmacyService> _logger;
        private readonly INominatedPharmacyClient _prescriptionTrackingClient;
        private readonly INominatedPharmacyConfig _config;

        public NominatedPharmacyService(
            ILogger<NominatedPharmacyService> logger,
            INominatedPharmacyClient prescriptionTrackingClient,
            INominatedPharmacyConfig config)
        {
            _logger = logger;
            _prescriptionTrackingClient = prescriptionTrackingClient;
            _config = config;
        }

        public async Task<GetNominatedPharmacyResult> GetNominatedPharmacy(string nhsNumber)
        {
            const string Dev = "DEV";
            const string Instance = "Instance";

            var request = new QUPA_IN000008UK02
            {
                Id = new Id
                {
                    Root = _config.MessageId,
                },
                CreationTime = new CreationTime
                {
                    Value = DateTime.Now.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture),
                },
                VersionCode = new VersionCode
                {
                    Code = "V3NPfIT4.2.00",
                },
                InteractionId = new InteractionId
                {
                    Extension = "QUPA_IN000008UK02",
                    Root = "2.16.840.1.113883.2.1.3.2.4.12",
                },
                ProcessingCode = new ProcessingCode
                {
                    Code = "P",
                },
                ProcessingModeCode = new ProcessingModeCode
                {
                    Code = "T",
                },
                AcceptAckCode = new AcceptAckCode
                {
                    Code = "NE",
                },
                CommunicationFunctionRcv = new CommunicationFunctionRcv
                {
                    Device = new Device
                    {
                        ClassCode = Dev,
                        DeterminerCode = Instance,
                        Id = new Id
                        {
                            Extension = _config.SpineAccreditedSystemIdTo,
                            Root = "1.2.826.0.1285.0.2.0.107",
                        },
                    },
                },
                CommunicationFunctionSnd = new CommunicationFunctionSnd
                {
                    Device = new Device
                    {
                        ClassCode = Dev,
                        DeterminerCode = Instance,
                        Id = new Id
                        {
                            Extension = _config.SpineAccreditedSystemIdFrom,
                            Root = "1.2.826.0.1285.0.2.0.107",
                        },
                    },
                },
                ControlActEvent = new ControlActEvent
                {
                    ClassCode = "CACT",
                    MoodCode = "EVN",
                    Author = new Author
                    {
                        TypeCode = "AUT",
                        AgentPersonSDS = new AgentPersonSDS
                        {
                            ClassCode = "AGNT",
                            Id = new Id
                            {
                                Extension = _config.SdsRoleId,
                                Root = "1.2.826.0.1285.0.2.0.67",
                            },
                            AgentPersonSDSInner = new AgentPersonSDSInner
                            {
                                ClassCode = "PSN",
                                DeterminerCode = Instance,
                                Id = new Id
                                {
                                    Extension = _config.SdsUserId,
                                    Root = "1.2.826.0.1285.0.2.0.65",
                                }
                            },
                            Part = new Part
                            {
                                TypeCode = "PART",
                                PartSDSRole = new PartSDSRole
                                {
                                    ClassCode = "ROL",
                                    Id = new Id
                                    {
                                        Extension = _config.SdsRole,
                                        Root = "1.2.826.0.1285.0.2.1.104",
                                    },
                                },
                            },
                        },
                    },
                    Author1 = new Author1
                    {
                        TypeCode = "AUT",
                        AgentSystemSDS = new AgentSystemSDS
                        {
                            ClassCode = "AGNT",
                            AgentSystemSDSInner = new AgentSystemSDSInner
                            {
                                ClassCode = Dev,
                                DeterminerCode = Instance,
                                Id = new Id
                                {
                                    Extension = "918999199235",
                                    Root = "1.2.826.0.1285.0.2.0.107",
                                },
                            },
                        },
                    },
                    Query = new Query
                    {
                        HistoricDataIndicator = new HistoricDataIndicator
                        {
                            SemanticsText = "HistoricDataIndicator",
                            Value = new Value
                            {
                                Code = "0",
                                CodeSystem = "2.16.840.1.113883.2.1.3.2.4.17.36",
                            },
                        },
                        PersonId = new PersonId
                        {
                            SemanticsText = "Person.id",
                            Value = new Value
                            {
                                Root = "2.16.840.1.113883.2.1.4.1",
                                Extension = nhsNumber,
                            },
                        },
                        RetrievalItem = new RetrievalItem
                        {
                            SemanticsText = "person.allData",
                        },
                    },
                }
            };

            var result = await _prescriptionTrackingClient.NominatedPharmacyGet(request);

            if (!result.HasSuccessResponse)
            {
                _logger.LogInformation("Error retrieving nominated pharmacy");
                return new GetNominatedPharmacyResult(result.StatusCode);
            }

            // Only use result if P1.
            var patientCareSection = result?.Body?.QUPA_IN000009UK03?.ControlActEvent?
                .Subject?.PDSResponse?.Subject?.PatientRole?.PatientPerson?
                .PlayedOtherProviderPatient?.SubjectOf?.PatientCareProvisionEvent;

            string odsCode = null;

            if (patientCareSection != null && patientCareSection.Code?._code == "P1")
            {
                odsCode = patientCareSection?.Performer?.AssignedEntity?.Id?.Extension;
            }

            var successResult = new GetNominatedPharmacyResult(result.StatusCode, odsCode);

            return successResult;
        }
    }
}
