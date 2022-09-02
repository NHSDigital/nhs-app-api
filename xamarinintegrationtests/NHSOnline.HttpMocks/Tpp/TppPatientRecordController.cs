using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Extensions;
using NHSOnline.HttpMocks.GpMedicalRecord;
using NHSOnline.HttpMocks.Tpp.Models;

namespace NHSOnline.HttpMocks.Tpp
{
    [Route("tpp")]
    public class TppPatientRecordController : TppBaseController
    {
        public TppPatientRecordController(IPatients patients) : base(patients)
        {
        }

        [HttpPost]
        [TppTypeHeader("ViewPatientOverview")]
        public IActionResult ViewPatientOverview([FromBody] ViewPatientOverview request)
        {
            if (!ModelState.IsValid || request?.PatientId == null)
            {
                return BadRequest(ModelState);
            }

            var patientId = request.PatientId;
            var patient = GetPatients().LookupById(request.PatientId);
            if (patient is TppPatient)
            {
                return ReturnXmlResult(patientId, BuildViewPatientOverviewResult());
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not a TPP patient");
        }

        [HttpPost]
        [TppTypeHeader("RequestPatientRecord")]
        public IActionResult RequestPatientRecord([FromBody] RequestPatientRecord request)
        {
            if (!ModelState.IsValid || request?.PatientId == null)
            {
                return BadRequest(ModelState);
            }

            var patientId = request.PatientId;
            var patient = GetPatients().LookupById(request.PatientId);
            if (patient is TppPatient tppPatient)
            {
                return ReturnXmlResult(patientId, BuildRequestPatientRecordResult());
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not a TPP patient");
        }

        [HttpPost]
        [TppTypeHeader("TestResultsView")]
        public IActionResult TestResultsView([FromBody] TestResultsView request)
        {
            if (!ModelState.IsValid || request?.PatientId == null)
            {
                return BadRequest(ModelState);
            }

            var patientId = request.PatientId;
            var patient = GetPatients().LookupById(request.PatientId);
            if (patient is TppPatient tppPatient)
            {
                return ReturnXmlResult(patientId, BuildTestResultsViewResult());
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not a TPP patient");
        }

        [HttpPost]
        [TppTypeHeader("RequestBinaryData")]
        public IActionResult RequestBinaryData([FromBody] RequestBinaryData request)
        {
            if (!ModelState.IsValid || request?.PatientId == null)
            {
                return BadRequest(ModelState);
            }

            var patientId = request.PatientId;
            var patient = GetPatients().LookupById(request.PatientId);
            if (patient is TppPatient tppPatient)
            {
                var behaviour = tppPatient.Behaviours.Get<ITppRecordsBehaviour>(() => new TppRecordsDefaultBehaviour());
                return behaviour.Behave(Request.HttpContext.Response.Headers, tppPatient);
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not a TPP patient");
        }

        private string BuildViewPatientOverviewResult()
        {
            var patientOverviewReply = new ViewPatientOverviewReply
            {
                Allergies = GenerateAllergiesList(),
                Drugs = GenerateMedicationsList(),
                CurrentRepeats = GenerateMedicationsList(),
                PastRepeats = GenerateMedicationsList(),
            };

            return XmlHelper.SerializeXml<ViewPatientOverviewReply>(patientOverviewReply);
        }

        private Collection<ViewPatientOverViewItem> GenerateMedicationsList()
        {
           return new Collection<ViewPatientOverViewItem>
            {
                new ViewPatientOverViewItem
                {
                    Id = 1.ToString(CultureInfo.InvariantCulture),
                    Date = GpMedicalRecordConstants.TenMonthsAgoDateString,
                    Value = "Penicillin"
                }
            };
        }

        private Collection<ViewPatientOverViewItem> GenerateAllergiesList()
        {
            return new Collection<ViewPatientOverViewItem>
            {
                new ViewPatientOverViewItem
                {
                    Id = 1.ToString(CultureInfo.InvariantCulture),
                    Date = GpMedicalRecordConstants.TenMonthsAgoDateString,
                    Value = "Hay Fever"
                }
            };
        }

        private string BuildRequestPatientRecordResult()
        {
            var requestPatientRecord = new RequestPatientRecordReply
            {
                Events = new Collection<Event>
                {
                    new Event
                    {
                        Date = GpMedicalRecordConstants.TenMonthsAgoDateString,
                        DoneBy = "Mr General NhsApp",
                        Location = "Kainos GP Demo Unit (General Practice)",
                        Items = new Collection<RequestPatientRecordItem>
                        {
                            new RequestPatientRecordItem
                            {
                                Details = "JPG: Blood-tests.jpg - some comments",
                                Type = "Attachment",
                                BinaryDataId = "123456433541"
                            },
                            new RequestPatientRecordItem
                            {
                                Details = "Benzoin tincture - 500 ml - use as directed",
                                Type = "Medication",
                            },
                        }
                    }
                }
            };

            return XmlHelper.SerializeXml<RequestPatientRecordReply>(requestPatientRecord);
        }

        private string BuildTestResultsViewResult()
        {
            var testResultsView = new TestResultsViewReply
            {
                Items = new Collection<TestResultsViewReplyItem>
                {
                    new TestResultsViewReplyItem
                    {
                        Id = "C435000000000000",
                        Value = "Anticoag Control (Warfarin)",
                        Description = "Pathology",
                        Date = GpMedicalRecordConstants.TenMonthsAgoDateString
                    }
                }
            };

            return XmlHelper.SerializeXml<TestResultsViewReply>(testResultsView);
        }
    }
}