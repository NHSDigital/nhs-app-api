extern alias r4;

using System;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;
using r4::Hl7.Fhir.Model;
using Period = Hl7.Fhir.Model.Period;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Mappers
{
    public class SecondaryCareWaitTimesMapper : SecondaryCareMapperBase, ISecondaryCareWaitTimesMapper
    {
        private const string PlannedWaitTimeExtensionUrl = "https://fhir.nhs.uk/StructureDefinition/Extension-PlannedWaitTime";

        private readonly ILogger<ISecondaryCareWaitTimesMapper> _logger;

        public SecondaryCareWaitTimesMapper(ILogger<SecondaryCareWaitTimesMapper> logger)
        {
            _logger = logger;
        }

        public WaitTimesResponse Map(Bundle bundle)
        {
            var response = new WaitTimesResponse();

            var carePlan = bundle.Entry
                .Select(x => x.Resource)
                .OfType<CarePlan>()
                .Single();

            foreach (var activity in carePlan.Activity)
            {
                response.WaitTimes.Add(MapActivityToWaitTime(activity));
            }

            return response;
        }

        private SecondaryCareWaitTimeItem MapActivityToWaitTime(CarePlan.ActivityComponent activity)
        {
            var referredDate = MapWaitTimeReferredDate(activity);
            var provider = MapProvider(activity);
            var serviceSpecialty = MapWaitTimeServiceSpecialty(activity);
            var plannedWaitTime = MapWaitTimePlannedWaitTime(activity);

            if (referredDate is null || provider is null || serviceSpecialty is null || plannedWaitTime is null)
            {
                return null;
            }

            var waitTime = new SecondaryCareWaitTimeItem
            {
                ReferredDate = referredDate.Value,
                PlannedWaitTime = plannedWaitTime.Value,
                ProviderName = provider,
                Speciality = serviceSpecialty
            };

            return waitTime;
        }

        private DateTimeOffset? MapWaitTimeReferredDate(CarePlan.ActivityComponent activity)
        {
            var referralDateString = (activity.Detail.Scheduled as Period)?.Start;

            if (string.IsNullOrWhiteSpace(referralDateString))
            {
                _logger.LogError("Failed to get WaitTime Referred Date from Scheduled Period");
                return null;
            }

            return DateTimeOffset.Parse(referralDateString, CultureInfo.InvariantCulture);
        }

        private string MapProvider(CarePlan.ActivityComponent activity)
        {
            var provider = activity.Detail.Performer.SingleOrDefault()?.Display;

            if (provider is null)
            {
                _logger.LogError("Failed to get Provider from Performer");
            }

            return provider;
        }

        private static string MapWaitTimeServiceSpecialty(CarePlan.ActivityComponent activity)
            => activity.Detail.Description;

        private decimal? MapWaitTimePlannedWaitTime(CarePlan.ActivityComponent activity)
        {
            if (!(activity.Detail.Scheduled is Period scheduledPeriod))
            {
                return null;
            }

            var plannedWaitTimeExtension =
                GetValueFromExtensionWithUrl<Duration>(
                    scheduledPeriod.Extension,
                    PlannedWaitTimeExtensionUrl,
                    _logger,
                    false);

            if (!(plannedWaitTimeExtension is Duration plannedWaitTime))
            {
                return null;
            }

            return plannedWaitTime.Value;
        }
    }
}