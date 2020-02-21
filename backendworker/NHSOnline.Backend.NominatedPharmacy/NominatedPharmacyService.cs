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
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.NominatedPharmacy.Soap;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;
using static NHSOnline.Backend.NominatedPharmacy.Soap.GetNominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyService : INominatedPharmacyService
    {
        private const string NominatedPharmacyCode = "P1";
        private const string MedicalApplianceCode = "P2";
        private const string DispensingDoctorCode = "P3";
        private const string DateOfBirthFormat = "yyyyMMdd";

        private readonly ILogger<NominatedPharmacyService> _logger;
        private readonly INominatedPharmacyClient _prescriptionTrackingClient;
        private readonly INominatedPharmacyConfigurationSettings _config;

        public NominatedPharmacyService(
            ILogger<NominatedPharmacyService> logger,
            INominatedPharmacyClient prescriptionTrackingClient,
            INominatedPharmacyConfigurationSettings config)
        {
            _logger = logger;
            _prescriptionTrackingClient = prescriptionTrackingClient;
            _config = config;
        }

        public async Task<GetNominatedPharmacyResult> GetNominatedPharmacy(string nhsNumber, CitizenIdUserSession cidUserSession)
        {
            _logger.LogEnter();

            var request = CreateRequest(nhsNumber);

            try
            {
                var result = await _prescriptionTrackingClient.NominatedPharmacyGet(request);

                if (!result.HasSuccessResponse)
                {
                    return new GetNominatedPharmacyResult.PharmacyRetrievalFailure(result.StatusCode);
                }

                LogIssueEvents(result);

                var nhsNumberReturned = GetNhsNumberReturned(result);

                if (!nhsNumber.RemoveWhiteSpace().Equals(nhsNumberReturned, StringComparison.Ordinal))
                {
                    return new GetNominatedPharmacyResult.NhsNumberSuperseded(result.StatusCode, nhsNumber, nhsNumberReturned);
                }

                var confidentialityCode = GetPatientRoleConfidentialityCode(result);

                if (confidentialityCode != null)
                {
                    return new GetNominatedPharmacyResult.ConfidentialAccount(result.StatusCode);
                }

                var knownPharmacyTypes = new[] { NominatedPharmacyCode, MedicalApplianceCode, DispensingDoctorCode };

                var patientCareProvisionEvents = GetPatientCareProvisionEvents(result, knownPharmacyTypes);

                var pharmacyCheck = CheckPharmacy(patientCareProvisionEvents);

                string odsCode;
                if (pharmacyCheck.IsValid)
                {
                    odsCode = GetOdsCode(pharmacyCheck);
                    if (!string.IsNullOrEmpty(odsCode))
                    {
                        _logger.LogInformation($"User retrieved nominated pharmacy with ods code: {odsCode}");
                    }
                    else
                    {
                        _logger.LogInformation("Pharmacy ods code check passed because no current pharmacy ods code exists");
                    }
                }
                else
                {
                    return new GetNominatedPharmacyResult.PharmacyChecksFailed(result.StatusCode);
                }

                var familyNameReturned = GetFamilyName(result);

                var dateOfBirthReturned = GetDateOfBirth(result);

                var personalDetailsCheck = CheckPersonalDetails(nhsNumber.RemoveWhiteSpace(), nhsNumberReturned, familyNameReturned, dateOfBirthReturned, cidUserSession);

                var pertinentSerialChangeNumber = GetPertinentSerialChangeNumber(result);

                var successResult = new GetNominatedPharmacyResponse(
                    result.StatusCode,
                    odsCode,
                    pertinentSerialChangeNumber,
                    pharmacyCheck.IsValid && personalDetailsCheck.IsValid,
                    pharmacyCheck.PharmacyType);

                if (!personalDetailsCheck.IsValid)
                {
                    return new GetNominatedPharmacyResult.PersonalChecksFailed(successResult);
                }

                return new GetNominatedPharmacyResult.Success(successResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to get the patient's nominated pharmacy");
                return new GetNominatedPharmacyResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private static string GetPertinentSerialChangeNumber(NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response> result)
        {
            return result.Body?.QUPAIN000009UK03?.ControlActEvent?.Subject?.PDSResponse
                ?.PertinentInformation?.PertinentSerialChangeNumber?.Value?.Value;
        }

        private static string GetDateOfBirth(NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response> result)
        {
            return result.Body?.QUPAIN000009UK03?.ControlActEvent?.Subject?.PDSResponse?.Subject
                ?.PatientRole?.PatientPerson?.BirthTime?.Value;
        }

        private static string GetFamilyName(NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response> result)
        {
            return result.Body?.QUPAIN000009UK03?.ControlActEvent?.Subject?.PDSResponse?.Subject
                ?.PatientRole?.PatientPerson?.COCTMT000203UK02PartOfWhole?.PartPerson?.Name?.Family;
        }

        private static string GetOdsCode(PharmacyCheck pharmacyCheck)
        {
            return pharmacyCheck.PatientCareProvisionEvent?.Performer?.AssignedEntity?.Id?.Extension;
        }

        private static IEnumerable<PatientCareProvisionEvent> GetPatientCareProvisionEvents(NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response> result, string[] knownPharmacyTypes)
        {
            return result?.Body?.QUPAIN000009UK03?.ControlActEvent
                ?.Subject?.PDSResponse?.Subject?.PatientRole?.PatientPerson?.PlayedOtherProviderPatients
                ?.Select(x => x.SubjectOf?.PatientCareProvisionEvent)
                .Where(y => knownPharmacyTypes.Contains(y?.Code?.Code));
        }

        private static ConfidentialityCode GetPatientRoleConfidentialityCode(NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response> result)
        {
            return result?.Body?.QUPAIN000009UK03?.ControlActEvent
                ?.Subject?.PDSResponse?.Subject?.PatientRole?.ConfidentialityCode;
        }

        private static string GetNhsNumberReturned(NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response> result)
        {
            return result.Body?.QUPAIN000009UK03?.ControlActEvent
                ?.Subject?.PDSResponse?.Subject?.PatientRole?.Id?.Extension;
        }

        private void LogIssueEvents(NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response> result)
        {
            var issueEvents =
                result.Body?.QUPAIN000009UK03?.ControlActEvent?.Reasons?.Select(x => x.JustifyingDetectedIssueEvent);

            if (issueEvents != null)
            {
                var issueEventsWithCode = issueEvents.Where(x => x.Code != null);
                foreach (var issue in issueEventsWithCode)
                {
                    _logger.LogInformation("JustifyingDetectedIssueEvent returned from PDS Trace " +
                                           $"with a code of [{issue.Code.Code}] and a displayName of [{issue.Code.DisplayName}]");
                }
            }
        }

        private QUPAIN000008UK02 CreateRequest(string nhsNumber)
        {
            const string dev = "DEV";
            const string instance = "INSTANCE";
            var messageId = Guid.NewGuid();

            return new QUPAIN000008UK02
            {
                Id = new Id
                {
                    Root = messageId.ToString(),
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
                CommunicationFunctionRcv = NominatedPharmacyUpdateRequest.CreateCommunicationFunctionRcv(_config.PdsTraceConfigurationSettings.ToAsid),
                CommunicationFunctionSnd = NominatedPharmacyUpdateRequest.CreateCommunicationFunctionSnd(_config.PdsTraceConfigurationSettings.FromAsid),
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
                            AgentSystemSDSInner = new AgentSystemSDSInner
                            {
                                ClassCode = dev,
                                DeterminerCode = instance,
                                Id = new Id
                                {
                                    Extension = _config.PdsTraceConfigurationSettings.FromAsid,
                                    Root = "1.2.826.0.1285.0.2.0.107",
                                },
                            },
                        },
                    },
                    Query = new QueryElement
                    {
                        HistoricDataIndicator = new HistoricDataIndicator
                        {
                            SemanticsText = "HistoricDataIndicator",
                            Value = new ValueElement
                            {
                                Code = "0",
                                CodeSystem = "2.16.840.1.113883.2.1.3.2.4.17.36",
                            },
                        },
                        PersonId = new PersonId
                        {
                            SemanticsText = "Person.id",
                            Value = new ValueElement
                            {
                                Root = "2.16.840.1.113883.2.1.4.1",
                                Extension = nhsNumber.RemoveWhiteSpace(),
                            },
                        },
                        RetrievalItems = new List<RetrievalItem>
                        {
                            new RetrievalItem
                            {
                                SemanticsText = "person.nameUsual",
                            },
                            new RetrievalItem
                            {
                                SemanticsText = "person.otherDemographics",
                            },
                            new RetrievalItem
                            {
                                SemanticsText = "pharmacy",
                            },
                            new RetrievalItem
                            {
                                SemanticsText = "person.confidentiality",
                            },
                        },
                    },
                },
                Xmlns = NominatedPharmacyNamespaces.Hl7,
            };
        }

        public async Task<UpdateNominatedPharmacyResult>
            UpdateNominatedPharmacy(NominatedPharmacyUpdate nominatedPharmacyUpdate)
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
                            nominatedPharmacyUpdate.PertinentSerialChangeNumber,
                            _config));

                if (!result.HasSuccessResponse)
                {
                    _logger.LogInformation($"The request to update a patients nominated pharmacy was unsuccessful. Response from spine: {result.RawResponse}");
                    return new UpdateNominatedPharmacyResult(result.StatusCode);
                }

                _logger.LogInformation($"Successfully completed request to update patient's nominated pharmacy. " +
                                       $"Spine ConversationId: { result.Response?.Header?.MessageHeader?.ConversationId }");

                return new UpdateNominatedPharmacyResult(result.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to update the patient's nominated pharmacy");
                return new UpdateNominatedPharmacyResult(HttpStatusCode.InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private PersonalDetailsCheck CheckPersonalDetails(string nhsNumber, string nhsNumberReturned,
            string familyNameReturned, string dateOfBirthReturned, CitizenIdUserSession cidUserSession)
        {
            var personalDetailsCheck = new PersonalDetailsCheck { IsValid = false };

            if (!string.IsNullOrEmpty(nhsNumberReturned))
            {
                if (!nhsNumberReturned.Equals(nhsNumber, StringComparison.Ordinal))
                {
                    _logger.LogInformation($"Returned NhsNumber {nhsNumberReturned} did not match expected NhsNumber {nhsNumber}");
                    return personalDetailsCheck;
                }
            }
            else
            {
                _logger.LogInformation("Could not extract nhsNumber from result of PDS Trace request");
                return personalDetailsCheck;
            }

            if (!string.IsNullOrEmpty(familyNameReturned))
            {
                if (!familyNameReturned.Equals(cidUserSession.FamilyName, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation($"Returned family name {familyNameReturned} " +
                                  $"did not match expected family name {cidUserSession.FamilyName}");
                    return personalDetailsCheck;
                }
            }
            else
            {
                _logger.LogInformation("Could not extract patient surname from result of PDS Trace request");
                return personalDetailsCheck;
            }

            if (DateTime.TryParseExact(dateOfBirthReturned, DateOfBirthFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var returnedDobParsed))
            {
                if (!returnedDobParsed.Equals(cidUserSession.DateOfBirth))
                {
                    _logger.LogInformation($"Returned date of birth {returnedDobParsed} " +
                                  $"did not match expected date of birth {cidUserSession.DateOfBirth}");
                    return personalDetailsCheck;
                }
            }
            else
            {
                _logger.LogInformation("Could not extract patient dateOfBirth from result of PDS Trace request");
                return personalDetailsCheck;
            }

            _logger.LogInformation("All Personal Details Checks have passed");
            personalDetailsCheck.IsValid = true;

            return personalDetailsCheck;
        }

        private PharmacyCheck CheckPharmacy(IEnumerable<PatientCareProvisionEvent> patientCareProvisionEvents)
        {
            const int pharmacyThreshold = 1;

            var patientCareSections = new Dictionary<string, PatientCareProvisionEvent>();

            patientCareSections.AddIfValueNotNull(NominatedPharmacyCode,
                patientCareProvisionEvents.FirstOrDefault(x => string.Equals(x.Code?.Code, NominatedPharmacyCode, StringComparison.Ordinal)));
            patientCareSections.AddIfValueNotNull(MedicalApplianceCode,
                patientCareProvisionEvents.FirstOrDefault(x => string.Equals(x.Code?.Code, MedicalApplianceCode, StringComparison.Ordinal)));
            patientCareSections.AddIfValueNotNull(DispensingDoctorCode,
                patientCareProvisionEvents.FirstOrDefault(x => string.Equals(x.Code?.Code, DispensingDoctorCode, StringComparison.Ordinal)));

            if (!patientCareSections.Any())
            {
                _logger.LogInformation("Patient does not have a nominated pharmacy set");
                return new PharmacyCheck { IsValid = true, PatientCareProvisionEvent = null };
            }
            else if (patientCareSections.Count > pharmacyThreshold)
            {
                StringBuilder logBuilder = new StringBuilder("Patient has multiple pharmacy types, ");

                var keys = patientCareSections.Select(x => x.Key).ToList();
                logBuilder.Append(JsonConvert.SerializeObject(keys));

                _logger.LogWarning(logBuilder.ToString());
                return new PharmacyCheck { IsValid = false, PatientCareProvisionEvent = null };
            }

            var patientCareSection = patientCareSections.First();
            switch (patientCareSection.Key)
            {
                case NominatedPharmacyCode:
                    _logger.LogInformation($"Patient has a valid {NominatedPharmacyCode} pharmacy");
                    return new PharmacyCheck
                    {
                        IsValid = true,
                        PatientCareProvisionEvent = patientCareSection.Value,
                        PharmacyType = NominatedPharmacyCode,
                    };
                case MedicalApplianceCode:
                case DispensingDoctorCode:
                    _logger.LogInformation(
                        $"Patient has a {patientCareSection.Key} pharmacy which is not a valid pharmacy type");
                    return new PharmacyCheck
                    {
                        IsValid = false,
                        PatientCareProvisionEvent = null,
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