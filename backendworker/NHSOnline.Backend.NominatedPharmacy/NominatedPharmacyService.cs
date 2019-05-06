using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.NominatedPharmacy.Soap;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support.Logging;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyService : INominatedPharmacyService
    {
        const string NominatedPharmacyCode = "P1";
        const string MedicalApplianceCode = "P2";
        const string DispensingDoctorCode = "P3";

        private readonly ILogger<NominatedPharmacyService> _logger;
        private readonly INominatedPharmacyClient _prescriptionTrackingClient;
        private readonly INominatedPharmacyConfig _config;
        private readonly IAuditor _auditor;

        public NominatedPharmacyService(
            ILogger<NominatedPharmacyService> logger,
            INominatedPharmacyClient prescriptionTrackingClient,
            INominatedPharmacyConfig config,
            IAuditor auditor
        )
        {
            _logger = logger;
            _prescriptionTrackingClient = prescriptionTrackingClient;
            _config = config;
            _auditor = auditor;
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

            _logger.LogEnter();
            try
            {
                var result = await _prescriptionTrackingClient.NominatedPharmacyGet(request);

                if (!result.HasSuccessResponse)
                {
                    _logger.LogError("Error retrieving nominated pharmacy");
                    return new GetNominatedPharmacyResult(result.StatusCode, false);
                }

                var knownPharmacyTypes = new[] { NominatedPharmacyCode, MedicalApplianceCode, DispensingDoctorCode };

                var patientCareProvisionEvents = result?.Body?.QUPA_IN000009UK03?.ControlActEvent
                    ?.Subject?.PDSResponse?.Subject?.PatientRole?.PatientPerson?.PlayedOtherProviderPatients
                    ?.Select(x => x.SubjectOf?.PatientCareProvisionEvent)
                    .Where(y => knownPharmacyTypes.Contains(y?.Code?._code));

                string odsCode = null;

                PharmacyCheck pharmacyCheck = new PharmacyCheck { IsValid = false };

                if (patientCareProvisionEvents != null && patientCareProvisionEvents.Any())
                {
                    pharmacyCheck = await CheckPharmacy(patientCareProvisionEvents);

                    if (pharmacyCheck.IsValid)
                    {
                        odsCode = pharmacyCheck.PatientCareProvisionEvent?.Performer?.AssignedEntity?.Id?.Extension;
                        _logger.LogInformation($"User retrieved nominated pharmacy with ods code: {odsCode}");
                    }
                    else
                    {
                        _logger.LogInformation("Invalid patient pharmacy or pharmacy combination");
                        return new GetNominatedPharmacyResult(result.StatusCode, false);
                    }
                }

                var pertinentSerialChangeNumber = result?.Body?.QUPA_IN000009UK03?.ControlActEvent?.Subject?.PDSResponse
                    ?.PertinentInformation?.PertinentSerialChangeNumber?.Value?._value;

                var successResult = new GetNominatedPharmacyResult(
                    result.StatusCode,
                    odsCode,
                    pertinentSerialChangeNumber,
                    pharmacyCheck.IsValid,
                    pharmacyCheck.PharmacyType);
                return successResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to get the patients nominated pharmacy");
                return new GetNominatedPharmacyResult(HttpStatusCode.InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<UpdateNominatedPharmacyResult> UpdateNominatedPharmacy(
            NominatedPharmacyUpdate nominatedPharmacyUpdate)
        {
            _logger.LogEnter();

            try
            {
                var result = await _prescriptionTrackingClient
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

        private async Task<PharmacyCheck> CheckPharmacy(
            IEnumerable<PatientCareProvisionEvent> patientCareProvisionEvents)
        {
            const int pharmacyThreshold = 1;

            var patientCareSections = new Dictionary<string, PatientCareProvisionEvent>();

            patientCareSections.AddIfValueNotNull(NominatedPharmacyCode,
                patientCareProvisionEvents.FirstOrDefault(x => x.Code?._code == NominatedPharmacyCode));
            patientCareSections.AddIfValueNotNull(MedicalApplianceCode,
                patientCareProvisionEvents.FirstOrDefault(x => x.Code?._code == MedicalApplianceCode));
            patientCareSections.AddIfValueNotNull(DispensingDoctorCode,
                patientCareProvisionEvents.FirstOrDefault(x => x.Code?._code == DispensingDoctorCode));

            if (!patientCareSections.Any())
            {
                _logger.LogInformation("Patient does not have a nominated pharmacy set");
                await _auditor.Audit(Constants.AuditingTitles.GetNominatedPharmacy,
                    "Patient does not have a nominated pharmacy set");
                return new PharmacyCheck { IsValid = false, PatientCareProvisionEvent = null };
            }
            else if (patientCareSections.Count > pharmacyThreshold)
            {
                StringBuilder logBuilder = new StringBuilder("Patient has multiple pharmacy types, ");

                var keys = patientCareSections.Select(x => x.Key).ToList();
                logBuilder.Append(JsonConvert.SerializeObject(keys));

                _logger.LogWarning(logBuilder.ToString());
                await _auditor.Audit(Constants.AuditingTitles.GetNominatedPharmacy, logBuilder.ToString());
                return new PharmacyCheck { IsValid = false, PatientCareProvisionEvent = null };
            }

            var patientCareSection = patientCareSections.First();
            switch (patientCareSection.Key)
            {
                case NominatedPharmacyCode:
                    _logger.LogInformation($"Patient has a valid {NominatedPharmacyCode} pharmacy");
                    await _auditor.Audit(Constants.AuditingTitles.GetNominatedPharmacy,
                        $"Successfully retrieved a valid {NominatedPharmacyCode} pharmacy");
                    return new PharmacyCheck
                    {
                        IsValid = true, 
                        PatientCareProvisionEvent = patientCareSection.Value,
                        PharmacyType = NominatedPharmacyCode,
                    };
                case MedicalApplianceCode:
                    _logger.LogInformation(
                        $"Patient has a {MedicalApplianceCode} pharmacy which is not a valid pharmacy type");
                    await _auditor.Audit(Constants.AuditingTitles.GetNominatedPharmacy,
                        $"Patient has a {MedicalApplianceCode} pharmacy");
                    return new PharmacyCheck
                    {
                        IsValid = false, 
                        PatientCareProvisionEvent = null,
                    };
                case DispensingDoctorCode:
                    _logger.LogInformation($"Patient has a valid {DispensingDoctorCode} pharmacy");
                    await _auditor.Audit(Constants.AuditingTitles.GetNominatedPharmacy,
                        $"Successfully retrieved a valid {DispensingDoctorCode} pharmacy");
                    return new PharmacyCheck
                    {
                        IsValid = true, 
                        PatientCareProvisionEvent = patientCareSection.Value,
                        PharmacyType = DispensingDoctorCode,
                    };
                default:
                    _logger.LogError("Unknown error when filtering nominated pharmacy");
                    return new PharmacyCheck
                    {
                        IsValid = false, 
                        PatientCareProvisionEvent = null
                    };
            }
        }
    }
}