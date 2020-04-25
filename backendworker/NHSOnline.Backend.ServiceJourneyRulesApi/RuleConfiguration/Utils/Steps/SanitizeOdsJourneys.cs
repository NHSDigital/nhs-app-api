using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Sanitization;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    internal class SanitizeOdsJourneys : ILoadStep
    {
        public string Description { get; } = "Sanitizing journeys properties intended to be rendered as HTML";
        public ProcessOrder Order { get; } = ProcessOrder.SanitizeOdsJourneys;

        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly ILogger<SanitizeOdsJourneys> _logger;

        public SanitizeOdsJourneys(IHtmlSanitizer htmlSanitizer, ILogger<SanitizeOdsJourneys> logger)
        {
            _htmlSanitizer = htmlSanitizer;
            _logger = logger;
        }

        public Task<bool> Execute(LoadContext context)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(context.MergedOdsJourneys, nameof(context.MergedOdsJourneys), ThrowError)
                .IsValid();

            SanitizeJourneys(context.MergedOdsJourneys);
            
            return Task.FromResult(true);
        }

        private void SanitizeJourneys(IDictionary<string, Journeys> mergedOdsJourneys)
        {
            foreach (var (_, journey) in mergedOdsJourneys)
            {
                var publicHealthNotifications = journey.HomeScreen?.PublicHealthNotifications;

                foreach (var notification in publicHealthNotifications ?? Enumerable.Empty<PublicHealthNotification>())
                {
                    notification.Body = _htmlSanitizer.SanitizeHtml(notification.Body);
                }
            }
        }
    }
}