using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.NominatedPharmacy.Soap;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyService : INominatedPharmacyService
    {
        const string NominatedPharmacyCodeP1 = "P1";
        const string NominatedPharmacyCodeP3 = "P3";

        private readonly ILogger<NominatedPharmacyService> _logger;
        private readonly INominatedPharmacyClient _prescriptionTrackingClient;
        private readonly INominatedPharmacyConfig _config;

        public NominatedPharmacyService(
            ILogger<NominatedPharmacyService> logger,
            INominatedPharmacyClient prescriptionTrackingClient,
            INominatedPharmacyConfig config
            )
        {
            _logger = logger;
            _prescriptionTrackingClient = prescriptionTrackingClient;
            _config = config;
        }

        public async Task<GetNominatedPharmacyResult> GetNominatedPharmacy(string nhsNumber)
        {
            const string Dev = "DEV";
            const string Instance = "INSTANCE";

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
                                Extension = _config.PersonSdsRoleId,
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
                                        Extension = _config.PartSdsRoleId,
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
                                    Extension = _config.SpineAccreditedSystemIdFrom,
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
                                Extension = nhsNumber.RemoveWhiteSpace(),
                            },
                        },
                        RetrievalItem = new RetrievalItem
                        {
                            SemanticsText = "person.allData",
                        },
                    },
                },
                Xmlns = NominatedPharmacyNamespaces.Hl7,
            };

            var result = await _prescriptionTrackingClient.NominatedPharmacyGet(request);

            if (!result.HasSuccessResponse)
            {
                _logger.LogError("Error retrieving nominated pharmacy");
                return new GetNominatedPharmacyResult(result.StatusCode);
            }

            // Only use result if P1.
            var patientCareProvisionEvents = result?.Body?.QUPA_IN000009UK03?.ControlActEvent
                ?.Subject?.PDSResponse?.Subject?.PatientRole?.PatientPerson?.PlayedOtherProviderPatients
                ?.Select(x => x.SubjectOf?.PatientCareProvisionEvent);

            string odsCode = null;
            string nominatedPharmacyType = null;

            if (patientCareProvisionEvents != null)
            {
                var patientCareP1Section = patientCareProvisionEvents.FirstOrDefault(x => x.Code?._code == NominatedPharmacyCodeP1);
                var patientCareP3Section = patientCareProvisionEvents.FirstOrDefault(x => x.Code?._code == NominatedPharmacyCodeP3);
                
                if (patientCareP1Section != null)
                {
                    odsCode = patientCareP1Section?.Performer?.AssignedEntity?.Id?.Extension;
                    nominatedPharmacyType = NominatedPharmacyCodeP1;
                    _logger.LogInformation($"User retrieved nominated pharmacy with ods code: { odsCode }");
                } 
                else if (patientCareP3Section != null)
                {
                    odsCode = patientCareP3Section?.Performer?.AssignedEntity?.Id?.Extension;
                    nominatedPharmacyType = NominatedPharmacyCodeP3;
                    _logger.LogInformation($"User retrieved dispensing doctor with ods code: { odsCode }");
                }
            }

            var pertinentSerialChangeNumber = result?.Body?.QUPA_IN000009UK03?.ControlActEvent?.Subject?.PDSResponse
                ?.PertinentInformation?.PertinentSerialChangeNumber?.Value?._value;

            var successResult = new GetNominatedPharmacyResult(
                result.StatusCode, 
                odsCode,
                nominatedPharmacyType,
                pertinentSerialChangeNumber
                );

            return successResult;
        }

        public async Task<UpdateNominatedPharmacyResult> UpdateNominatedPharmacy(NominatedPharmacyUpdate nominatedPharmacyUpdate)
        {
            _logger.LogEnter();

            try
            {
                var result =  await _prescriptionTrackingClient
                    .UpdateNominatedPharmacy(
                        new NominatedPharmacyUpdateRequest(
                            nominatedPharmacyUpdate.NhsNumber,
                            nominatedPharmacyUpdate.HasExistingNominatedPharmacy,
                            nominatedPharmacyUpdate.UpdatedOdsCode,
                            nominatedPharmacyUpdate.PertinentSerialChangeNumber));

                if (!result.HasSuccessResponse)
                {
                    _logger.LogInformation("The request to update a patients nominated pharmacy was unsuccessful");
                    return new UpdateNominatedPharmacyResult(result.StatusCode);
                }

                _logger.LogInformation("Successfully completed request to update patient's nominated pharmacy");
                return new UpdateNominatedPharmacyResult(result.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to update the patients nominated pharmacy");
                return new UpdateNominatedPharmacyResult(HttpStatusCode.InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}