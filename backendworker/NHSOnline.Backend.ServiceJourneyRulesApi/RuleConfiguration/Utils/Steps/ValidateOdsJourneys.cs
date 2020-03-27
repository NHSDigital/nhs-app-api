using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    internal class ValidateOdsJourneys : IValidatorStep, ILoadStep
    {
        public string Description { get; } = "Validating Ods journeys";
        public ProcessOrder Order { get; } = ProcessOrder.ValidateOdsJourneys;

        private readonly ILogger _logger;

        private readonly JourneysValidator _journeysValidator =
            new JourneysValidator()
                .Add(journeys => journeys.Appointments?.Provider != null, "journeys.Appointments.Provider")
                .Add(journeys => journeys.CdssAdvice?.Provider != null, "journeys.CdssAdvice.Provider")
                .Add(journeys => journeys.CdssAdmin?.Provider != null, "journeys.CdssAdmin.Provider")
                .Add(journeys => journeys.MedicalRecord?.Provider != null, "journeys.MedicalRecord.Provider")
                .Add(journeys => journeys.Prescriptions?.Provider != null, "journeys.Prescriptions.Provider")
                .Add(journeys => journeys.NominatedPharmacy.HasValue, "journeys.NominatedPharmacy")
                .Add(journeys => journeys.Notifications.HasValue, "journeys.Notifications")
                .Add(journeys => journeys.Messaging.HasValue, "journeys.Messaging")
                .Add(journeys => journeys.UserInfo.HasValue, "journeys.UserInfo")
                .Add(journeys => journeys.SilverIntegrations?.SecondaryAppointments != null,
                    "journeys.SilverIntegrations.SecondaryAppointments")
                .Add(journeys => journeys.SilverIntegrations?.Messages != null,
                    "journeys.SilverIntegrations.Messages")
                .Add(journeys => journeys.SilverIntegrations?.Consultations != null,
                    "journeys.SilverIntegrations.Consultations")
                .Add(journeys => journeys.Documents.HasValue, "journeys.Documents")
                .Add(journeys => journeys.Im1Messaging?.IsEnabled != null, "journeys.im1Messaging.isEnabled")
                .Add(journeys => journeys.Im1Messaging?.CanDeleteMessages != null, "journeys.im1Messaging.canDeleteMessages")
                .Add(journeys => journeys.Supplier != Supplier.Unknown, "journeys.Supplier")
                .Add(journeys =>
                {
                    var homeScreen = journeys.HomeScreen;

                    if (homeScreen == null) return true;

                    return homeScreen.PublicHealthNotifications?.Any() ?? false;
                }, "journeys.HomeScreen.PublicHealthNotifications");

        public ValidateOdsJourneys(ILogger<ValidateOdsJourneys> logger)
        {
            _logger = logger;
        }

        public Task<bool> Execute(ConfigurationContext context)
        {
            return Execute((LoadContext) context);
        }

        public Task<bool> Execute(LoadContext context)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(context, nameof(context), ThrowError)
                .IsNotNull(context?.MergedOdsJourneys, nameof(context.MergedOdsJourneys), ThrowError)
                .IsValid();

            if (Validate(context.MergedOdsJourneys))
            {
                return Task.FromResult(true);
            }

            _logger.LogCritical("Error validating merged journeys. See output above for specific errors.");
            return Task.FromResult(false);
        }

        private bool Validate(IDictionary<string, Journeys> odsJourneys)
        {
            var isValid = true;
            foreach (var (odsCode, journeys) in odsJourneys)
            {
                var errorList = _journeysValidator.GetAnyInvalidProperties(journeys);

                if (errorList.Any())
                {
                    _logger.LogError($"Not all journey types have been defined for '{odsCode}'. Missing Values:\n" +
                                     string.Join("\n", errorList));
                    isValid = false;
                }
                else
                {
                    _logger.LogDebug($"Validation successful for '{odsCode}'");
                }
            }
            return isValid;
        }
    }
}