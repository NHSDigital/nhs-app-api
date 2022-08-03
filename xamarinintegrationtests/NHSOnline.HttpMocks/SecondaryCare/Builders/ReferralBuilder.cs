using System;
using System.Collections.Generic;
using System.Globalization;
using NHSOnline.HttpMocks.Support;
using Hl7.Fhir.Model;
using Extension = Hl7.Fhir.Model.Extension;
using ResourceReference = Hl7.Fhir.Model.ResourceReference;

namespace NHSOnline.HttpMocks.SecondaryCare.Builders
{
    public class ReferralBuilder
    {
        private int? _referredDaysAgo;
        private ServiceSpecialty _serviceSpecialty;
        private ReferralStatus? _referralStatus;
        private int? _dueDateInDays;
        private ServiceProvider _serviceProvider = ServiceProvider.eRS;

        public ReferralBuilder WithReferredDateDaysAgo(int days)
        {
            _referredDaysAgo = -days;
            return this;
        }

        public ReferralBuilder WithSpecialty(ServiceSpecialty specialty)
        {
            _serviceSpecialty = specialty;
            return this;
        }

        public ReferralBuilder WithReferralStatus(ReferralStatus status)
        {
            _referralStatus = status;
            return this;
        }

        public ReferralBuilder WithDueDateInDays(int days)
        {
            _dueDateInDays = days;
            return this;
        }

        public ReferralBuilder WithMissedDueDateDaysAgo(int days)
            => WithDueDateInDays(-days);

        public ReferralBuilder WithProvider(ServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
            return this;
        }

        public CarePlan.ActivityComponent Build()
        {
            if (_referredDaysAgo is null)
            {
                throw new InvalidOperationException("Referred date has not been provided. User build method `WithReferredDateDaysAgo`");
            }

            if (_referralStatus is null)
            {
                throw new InvalidOperationException("Referral status has not been provided. User build method `WithReferralStatus`");
            }

            var ubrn = UbrnGenerator.NewUbrn();

            var portalUrl = string.Format(CultureInfo.InvariantCulture, Constants.ProviderUrlMappings[_serviceProvider], ubrn);
            var uri = new Uri(portalUrl);

            var scheduledPeriod = _dueDateInDays is null
                ? FhirBuilderHelpers.GetScheduledPeriod(_referredDaysAgo.Value)
                : FhirBuilderHelpers.GetScheduledPeriod(_referredDaysAgo.Value, _dueDateInDays.Value);

            var portalLinkExtension = FhirBuilderHelpers.GetPortalLinkExtension(
                _serviceProvider.ToString(),
                uri);

            var referralStatus = FhirBuilderHelpers.GetReferralStateExtension(_referralStatus.Value.ToString());

            var serviceSpecialty = Constants.ServiceSpecialtyMappings[_serviceSpecialty];

            return new CarePlan.ActivityComponent
            {
                Reference = FhirBuilderHelpers.GetReferralIdentity(ubrn),
                Detail = new CarePlan.DetailComponent
                {
                    Kind = CarePlan.CarePlanActivityKind.ServiceRequest,
                    Scheduled = scheduledPeriod,
                    Description = serviceSpecialty,
                    Extension = new List<Extension>
                    {
                        portalLinkExtension,
                        referralStatus,
                    },
                },
            };
        }
    }
}