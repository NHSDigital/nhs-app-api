extern alias r4;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;
using r4::Hl7.Fhir.Model;
using AppointmentStatus = r4::Hl7.Fhir.Model.Appointment.AppointmentStatus;
using Date = Hl7.Fhir.Model.Date;
using Code = Hl7.Fhir.Model.Code;
using Coding = Hl7.Fhir.Model.Coding;
using Extension = Hl7.Fhir.Model.Extension;
using FhirUrl = Hl7.Fhir.Model.FhirUrl;
using Period = Hl7.Fhir.Model.Period;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Mappers
{
    public class SecondaryCareSummaryMapper : ISecondaryCareSummaryMapper
    {
        private const string PortalLinkResourceUrl = "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link";
        private const string ReferralStateResourceUrl = "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State";
        private const string ReviewDueDateResourceUrl = "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ReviewDueDate";
        private const string AppointmentStatusResourceUrl = "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status";

        private readonly ILogger<ISecondaryCareSummaryMapper> _logger;

        public SecondaryCareSummaryMapper(ILogger<SecondaryCareSummaryMapper> logger)
        {
            _logger = logger;
        }

        public ISummaryResponse Map(Bundle bundle)
        {
            var response = new SummaryResponse();

            var carePlans = bundle.Entry
                .Select(x => x.Resource)
                .OfType<CarePlan>();

            foreach (var carePlan in carePlans)
            {
                foreach (var activity in carePlan.Activity)
                {
                    switch (activity.Detail?.Kind)
                    {
                        case CarePlan.CarePlanActivityKind.ServiceRequest:
                        {
                            var referral = MapActivityToReferral(activity);

                            if (referral is null)
                            {
                                return null;
                            }

                            if (referral.IsInReview && !referral.IsOverdue)
                            {
                                response.AddReferralInReviewNotOverdue(referral);
                                break;
                            }

                            response.AddActionableReferral(referral);
                            break;
                        }
                        case CarePlan.CarePlanActivityKind.Appointment:
                        {
                            var appointment = MapActivityToUpcomingAppointment(activity);

                            if (appointment is null)
                            {
                                return null;
                            }

                            if (appointment.IsConfirmed)
                            {
                                response.AddConfirmedAppointment(appointment);
                                break;
                            }

                            response.AddActionableAppointment(appointment);
                            break;
                        }
                    }
                }
            }

            response.Sort();

            return response;
        }

        private Referral MapActivityToReferral(CarePlan.ActivityComponent activity)
        {
            var referralId = MapReferralId(activity);
            var referredDate = MapReferralReferredDate(activity);
            var status = MapReferralStatus(activity);
            var (provider, deepLink) = MapProviderAndDeepLink(activity);
            var serviceSpecialty = MapReferralServiceSpecialty(activity);
            var dueDate = MapReferralDueDate(activity);

            if (referralId is null || referredDate is null || status is null || deepLink is null || provider is null)
            {
                return null;
            }

            var referral = new Referral
            {
                ReferralId = referralId,
                ReferredDateTime = referredDate.Value,
                Status = status,
                DeepLinkUrl = deepLink,
                Provider = provider,
                ServiceSpecialty = serviceSpecialty,
                ReviewDueDate = dueDate,
            };

            return referral;
        }

        private UpcomingAppointment MapActivityToUpcomingAppointment(CarePlan.ActivityComponent activity)
        {
            var appointmentDate = MapUpcomingAppointmentDate(activity);
            var status = MapUpcomingAppointmentStatus(activity);
            var location = MapUpcomingAppointmentLocation(activity);
            var (provider, deepLink) = MapProviderAndDeepLink(activity);

            if (status is null || location is null || deepLink is null || provider is null)
            {
                return null;
            }

            var upcomingAppointment = new UpcomingAppointment
            {
                AppointmentDateTime = appointmentDate,
                AppointmentStatus = status,
                LocationDescription = location,
                Provider = provider,
                DeepLinkUrl = deepLink,
            };

            return upcomingAppointment;
        }

        private string MapReferralId(CarePlan.ActivityComponent activity)
        {
            var id = activity.Reference?.Identifier?.Value;

            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogError("Failed to get Referral Booking Reference from Reference Identifier");
                return null;
            }

            return id;
        }

        private DateTimeOffset? MapReferralReferredDate(CarePlan.ActivityComponent activity)
        {
            var referralDateString = (activity.Detail.Scheduled as Period)?.Start;

            if (string.IsNullOrWhiteSpace(referralDateString))
            {
                _logger.LogError("Failed to get Referral Referred Date from Scheduled Period");
                return null;
            }

            return DateTimeOffset.Parse(referralDateString, CultureInfo.InvariantCulture);
        }

        private string MapReferralStatus(CarePlan.ActivityComponent activity)
            => (GetValueFromExtensionWithUrl<Coding>(activity.Detail.Extension, ReferralStateResourceUrl) as Coding)?.Code;

        private (string provider, string deepLink) MapProviderAndDeepLink(CarePlan.ActivityComponent activity)
        {
            var portalLinkExtension = GetExtensionByUrl(activity.Detail.Extension, PortalLinkResourceUrl);

            if (portalLinkExtension is null)
            {
                return (null, null);
            }

            var provider = (GetValueFromExtensionWithUrl<Code>(portalLinkExtension.Extension, "client-id") as Code)?.Value;
            var deepLink = (portalLinkExtension.Value as FhirUrl)?.Value;

            if (provider is null)
            {
                _logger.LogError("Failed to get Provider from extensions");
            }

            if (deepLink is null)
            {
                _logger.LogError("Failed to get Deep Link from extensions");
            }

            return (provider, deepLink);
        }

        private static string MapReferralServiceSpecialty(CarePlan.ActivityComponent activity)
            => activity.Detail.Description;

        private DateTimeOffset? MapReferralDueDate(CarePlan.ActivityComponent activity)
        {
            if (!(activity.Detail.Scheduled is Period scheduledPeriod))
            {
                return null;
            }

            var dateExtension = GetValueFromExtensionWithUrl<Date>(scheduledPeriod.Extension, ReviewDueDateResourceUrl, false);

            if (!(dateExtension is Date dueDate))
            {
                return null;
            }

            return DateTimeOffset.Parse(dueDate.Value, CultureInfo.InvariantCulture);
        }

        private DateTimeOffset? MapUpcomingAppointmentDate(CarePlan.ActivityComponent activity)
        {
            var appointmentDateString = (activity.Detail.Scheduled as Period)?.Start;

            if (string.IsNullOrWhiteSpace(appointmentDateString))
            {
                return null;
            }

            return DateTimeOffset.Parse(appointmentDateString, CultureInfo.CurrentCulture);
        }

        private string MapUpcomingAppointmentStatus(CarePlan.ActivityComponent activity)
        {
            var status = (GetValueFromExtensionWithUrl<Coding>(activity.Detail.Extension, AppointmentStatusResourceUrl) as Coding)?.Code;

            if (string.IsNullOrWhiteSpace(status))
            {
                _logger.LogError("Failed to get Upcoming Appointment Status from extensions");
                return null;
            }

            if (!string.Equals(status, AppointmentStatus.Booked.GetLiteral(), StringComparison.Ordinal) &&
                !string.Equals(status, AppointmentStatus.Cancelled.GetLiteral(), StringComparison.Ordinal))
            {
                // Convert all pending change/cancel statuses to one value as they are handled the same
                status = AppointmentStatus.Pending.GetLiteral();
            }

            return status;
        }

        private string MapUpcomingAppointmentLocation(CarePlan.ActivityComponent activity)
        {
            var location = activity.Detail.Description;

            if (string.IsNullOrWhiteSpace(location))
            {
                _logger.LogError("Failed to get Upcoming Appointment Location from Description");
            }

            return location;
        }

        private object GetValueFromExtensionWithUrl<T>(IList<Extension> extensions, string url, bool logError = true)
        {
            var extension = GetExtensionByUrl(extensions, url, logError);

            if (extension?.Value is T value)
            {
                return value;
            }

            if (logError)
            {
                _logger.LogError("Expected Extension value to be of type {T} but was {TypeName}", typeof(T), extension?.Value.GetType());
            }

            return null;
        }

        private Extension GetExtensionByUrl(IList<Extension> extensions, string url, bool logErrors = true)
        {
            try
            {
                return extensions.First(e => string.Equals(e.Url, url, StringComparison.Ordinal));
            }
            catch (InvalidOperationException e)
            {
                if (logErrors)
                {
                    _logger.LogError(e, "Could not find Extension of type {Url}", url);
                }
            }

            return null;
        }
    }
}