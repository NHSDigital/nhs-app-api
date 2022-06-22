using System;
using System.Collections.Generic;
using System.Globalization;
using Hl7.Fhir.Utility;
using NHSOnline.HttpMocks.Support;
using Hl7.Fhir.Model;
using Extension = Hl7.Fhir.Model.Extension;

namespace NHSOnline.HttpMocks.SecondaryCare.Builders
{
    public class UpcomingAppointmentBuilder
    {
        private Appointment.AppointmentStatus? _appointmentStatus;
        private string? _locationDescription;
        private ServiceSpecialty? _serviceSpecialty;
        private ServiceProvider? _serviceProvider;
        private int? _appointmentInDays;
        private string? _providerUbrn;

        public UpcomingAppointmentBuilder WithAppointmentStatus(Appointment.AppointmentStatus status)
        {
            _appointmentStatus = status;
            return this;
        }

        public UpcomingAppointmentBuilder WithLocationDescription(string description)
        {
            _locationDescription = description;
            return this;
        }

        public UpcomingAppointmentBuilder WithSpecialty(ServiceSpecialty specialty)
        {
            _serviceSpecialty = specialty;
            return this;
        }

        public UpcomingAppointmentBuilder WithProvider(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            return this;
        }

        public UpcomingAppointmentBuilder WithProviderUbrn(string providerUbrn)
        {
            _providerUbrn = providerUbrn;
            return this;
        }

        public UpcomingAppointmentBuilder WithAppointmentDateInDays(int? days)
        {
            _appointmentInDays = days;
            return this;
        }

        public CarePlan.ActivityComponent Build()
        {
            if (_appointmentStatus is null)
            {
                throw new InvalidOperationException("Appointment status has not been provided. User build method `WithAppointmentStatus`");
            }

            if (string.IsNullOrEmpty(_locationDescription) && _serviceSpecialty is null)
            {
                throw new InvalidOperationException("Location description or Service specialty has not been provided. User build method `WithLocationDescription` or `WithServiceSpecialty`");
            }

            if (_serviceProvider is null)
            {
                throw new InvalidOperationException("Service provider has not been provided. User build method `WithProvider`");
            }

            var ubrn = string.IsNullOrEmpty(_providerUbrn)
                ? UbrnGenerator.NewUbrn()
                : _providerUbrn;

            var portalUrl = string.Format( CultureInfo.InvariantCulture, Constants.ProviderUrlMappings[_serviceProvider.Value], ubrn);
            var uri = new Uri(portalUrl);

            var portalLinkExtension = FhirBuilderHelpers.GetPortalLinkExtension(
                _serviceProvider.Value.ToString(), uri);

            var appointmentStatusExtension = FhirBuilderHelpers.GetAppointmentStatusExtension(
                _appointmentStatus.Value.GetLiteral());

            var activity = new CarePlan.ActivityComponent
            {
                Detail = new CarePlan.DetailComponent
                {
                    Kind = CarePlan.CarePlanActivityKind.Appointment,
                    Description = GetDetailDescription(),
                    Extension = new List<Extension>
                    {
                        portalLinkExtension,
                        appointmentStatusExtension,
                    },
                },
            };

            if (_appointmentInDays is not null)
            {
                activity.Detail.Scheduled = FhirBuilderHelpers.GetScheduledPeriod(_appointmentInDays.Value);
            }

            return activity;
        }

        private string GetDetailDescription()
        {
            if (!string.IsNullOrEmpty(_locationDescription))
            {
                return _locationDescription;
            }

            return _serviceSpecialty is null or ServiceSpecialty.None
                ? string.Empty
                : _serviceSpecialty.Value.ToString();
        }
    }
}