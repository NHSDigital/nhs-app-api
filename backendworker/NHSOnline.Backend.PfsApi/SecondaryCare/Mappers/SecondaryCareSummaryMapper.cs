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
using OperationOutcome = Hl7.Fhir.Model.OperationOutcome;
using Period = Hl7.Fhir.Model.Period;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Mappers
{
    public class SecondaryCareSummaryMapper : ISecondaryCareSummaryMapper
    {
        private const string PortalLinkResourceUrl = "https://fhir.nhs.uk/StructureDefinition/Extension-Portal-Link";
        private const string ReferralStateResourceUrl = "https://fhir.nhs.uk/StructureDefinition/Extension-eRS-ServiceRequest-State";
        private const string ReviewDueDateResourceUrl = "https://fhir.nhs.net/ReviewDueDate";
        private const string AppointmentStatusResourceUrl = "https://fhir.nhs.uk/StructureDefinition/Extension-Appointment-Status";
        private const string InReviewStatus = "InReview";

        private readonly ILogger<ISecondaryCareSummaryMapper> _logger;

        public SecondaryCareSummaryMapper(ILogger<SecondaryCareSummaryMapper> logger)
        {
            _logger = logger;
        }

        public SummaryResponse Map(Bundle bundle)
        {
            var referralsInReview = new List<Referral>();
            var referralsNotInReview = new List<Referral>();
            var confirmedAppointments = new List<UpcomingAppointment>();
            var unconfirmedAppointments = new List<UpcomingAppointment>();

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

                            if (referral.Status == InReviewStatus)
                            {
                                referralsInReview.Add(referral);
                                break;
                            }

                            referralsNotInReview.Add(referral);
                            break;
                        }
                        case CarePlan.CarePlanActivityKind.Appointment:
                        {
                            var appointment = MapActivityToUpcomingAppointment(activity);

                            if (appointment is null)
                            {
                                return null;
                            }

                            if (appointment.AppointmentDateTime != null)
                            {
                                if (string.Equals(appointment.AppointmentStatus,
                                        AppointmentStatus.Booked.GetLiteral(),
                                        StringComparison.OrdinalIgnoreCase)
                                    || string.Equals(appointment.AppointmentStatus,
                                         AppointmentStatus.Cancelled.GetLiteral(),
                                         StringComparison.OrdinalIgnoreCase)
                                   )
                                {
                                    confirmedAppointments.Add(appointment);
                                    break;
                                }
                                unconfirmedAppointments.Add(appointment);
                            }
                            else
                            {
                                unconfirmedAppointments.Add(appointment);
                            }

                            break;
                        }
                    }
                }
            }

            return new SummaryResponse
            {
                ReferralsNotInReview = referralsNotInReview
                    .OrderBy(r => r.ReferredDateTime)
                    .ToList(),
                ReferralsInReview = referralsInReview
                    .OrderBy(r => r.ReferredDateTime)
                    .ToList(),
                UnconfirmedAppointments = unconfirmedAppointments
                    .OrderBy(a => a.AppointmentDateTime)
                    .ToList(),
                ConfirmedAppointments = confirmedAppointments
                    .OrderBy(a => string.Equals(a.AppointmentStatus, AppointmentStatus.Cancelled.GetLiteral(), StringComparison.OrdinalIgnoreCase))
                    .ThenBy(a => a.AppointmentDateTime)
                    .ToList(),
            };
        }

        private Referral MapActivityToReferral(CarePlan.ActivityComponent activity)
        {
            var referralId = MapReferralId(activity);
            var referredDate = MapReferralReferredDate(activity);
            var status = MapReferralStatus(activity);
            var organisation = MapReferralOrganisation(activity);
            var (provider, deepLink) = MapProviderAndDeepLink(activity);
            var serviceSpecialty = MapReferralServiceSpecialty(activity);
            var dueDate = MapReferralDueDate(activity);

            if (referralId is null || referredDate is null || status is null || organisation is null || deepLink is null || provider is null)
            {
                return null;
            }

            var referral = new Referral
            {
                ReferralId = referralId,
                ReferredDateTime = referredDate.Value,
                Status = status,
                ReferrerOrganisation = organisation,
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

        private string MapReferralOrganisation(CarePlan.ActivityComponent activity)
        {
            try
            {
                return activity.Detail.Performer
                    .First(p => string.Equals(p.Type, "Organization", StringComparison.Ordinal))
                    .Display;
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Could not find Referral Organisation in list of Performers");
            }

            return null;
        }

        private (string provider, string deepLink) MapProviderAndDeepLink(CarePlan.ActivityComponent activity)
        {
            var portalLinkExtension = GetExtensionByUrl(activity.Detail.Extension, PortalLinkResourceUrl);

            if (portalLinkExtension is null)
            {
                return (null, null);
            }

            var provider = (GetValueFromExtensionWithUrl<Code>(portalLinkExtension.Extension, "code") as Code)?.Value;
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