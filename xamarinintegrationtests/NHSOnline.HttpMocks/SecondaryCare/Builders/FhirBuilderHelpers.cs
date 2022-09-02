using System.Collections.Generic;
using System.Globalization;
using Hl7.Fhir.Model;
using NHSOnline.HttpMocks.GpMedicalRecord;
using CarePlan = Hl7.Fhir.Model.CarePlan;
using Code = Hl7.Fhir.Model.Code;
using Date = Hl7.Fhir.Model.Date;
using DateTime = System.DateTime;

namespace NHSOnline.HttpMocks.SecondaryCare.Builders
{
    public static class FhirBuilderHelpers
    {
        public static ResourceReference GetNhsNumberReference(string nhsNumber) => new()
        {
            Identifier = new Identifier
            {
                System = "https://fhir.nhs.uk/Id/nhs-number",
                Value = nhsNumber,
            },
        };

        public static ResourceReference GetReferralIdentity(string ubrn) => new()
        {
            Type = CarePlan.CarePlanActivityKind.ServiceRequest.ToString(),
            Identifier = new Identifier
            {
                System = "https://fhir.nhs.uk/Id/UBRN",
                Value = ubrn,
            }
        };

        public static Extension GetPortalLinkExtension(string provider, System.Uri  portalUrl) => new()
        {
            Url = "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link",
            Value = new FhirUrl(portalUrl),
            Extension = new List<Extension>
            {
                new Extension
                {
                    Url = "client-id",
                    Value = new Code(provider),
                },
            },
        };

        public static Extension GetReferralStateExtension(string referralStatus) => new()
        {
            Url = "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State",
            Value = new Coding
            {
                System = "https://fhir.nhs.uk/CodeSystem/eRS-ReferralState",
                Code = referralStatus,
            },
        };

        public static Extension GetAppointmentStatusExtension(string appointmentStatus) => new()
        {
            Url = "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status",
            Value = new Coding
            {
                System = "http://hl7.org/fhir/appointmentstatus",
                Code = appointmentStatus,
            },
        };

        public static Extension GetErrorExtension(string clientId) => new()
        {
            Url = "https://fhir.nhs.uk/StructureDefinition/ExtensionErrorSource",
            Value = new Code(clientId)
        };

        public static Period GetScheduledPeriod(int startDaysOffset)
        {
            var period = new Period
            {
                Start = DateTime.UtcNow.AddDays(startDaysOffset).ToString(GpMedicalRecordConstants.FhirDateTimeFormat, CultureInfo.InvariantCulture),
            };

            return period;
        }

        public static Period GetScheduledPeriod(int startDaysOffset, int endDaysOffset)
        {
            var period = GetScheduledPeriod(startDaysOffset);

            var date = DateTime.UtcNow.AddDays(endDaysOffset);
            period.Extension.Add(GetReviewDueDate(date.Year, date.Month, date.Day));

            return period;
        }

        private static Extension GetReviewDueDate(int year, int month, int day) => new()
        {
            Url = "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ReviewDueDate",
            Value = new Date(year, month, day)
        };

        public static ResourceReference GetOrganisationPerformer(string organisation) => new ResourceReference
        {
            Type = "Organization",
            Display = organisation
        };
    }
}