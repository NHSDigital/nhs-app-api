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
        private static readonly JourneysValidator JourneysValidator = CreateJourneysValidator();

        public string Description { get; } = "Validating Ods journeys";
        public ProcessOrder Order { get; } = ProcessOrder.ValidateOdsJourneys;

        private readonly ILogger _logger;

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
                var errorList = JourneysValidator.GetAnyInvalidProperties(odsCode, journeys);

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

        private static JourneysValidator CreateJourneysValidator()
        {
            var journeysValidator = new JourneysValidator()
                .Add(journeys => journeys.Appointments?.Provider != null, "journeys.Appointments.Provider")
                .Add(journeys => journeys.CdssAdvice?.Provider != null, "journeys.CdssAdvice.Provider")
                .Add(journeys => journeys.CdssAdmin?.Provider != null, "journeys.CdssAdmin.Provider")
                .Add(journeys => journeys.MedicalRecord?.Provider != null, "journeys.MedicalRecord.Provider")
                .Add(journeys => journeys.Prescriptions?.Provider != null, "journeys.Prescriptions.Provider")
                .Add(journeys => journeys.NominatedPharmacy.HasValue, "journeys.NominatedPharmacy")
                .Add(journeys => journeys.Notifications.HasValue, "journeys.Notifications")
                .Add(journeys => journeys.Messaging.HasValue, "journeys.Messaging")
                .Add(journeys => journeys.UserInfo.HasValue, "journeys.UserInfo")
                .Add(journeys => journeys.Documents.HasValue, "journeys.Documents")
                .Add(journeys => journeys.SupportsLinkedProfiles.HasValue, "journeys.SupportsLinkedProfiles")
                .Add(ValidSupplier, "journeys.Supplier");

            AddSilverIntegrationValidations(journeysValidator);
            AddIm1MessagingValidations(journeysValidator);
            AddHomeScreenValidations(journeysValidator);

            return journeysValidator;

            static bool ValidSupplier(string odsCode, Journeys journeys)
            {
                if (journeys.Supplier.HasValue && journeys.Supplier != Supplier.Unknown)
                {
                    return true;
                }

                return journeys.Supplier == null && odsCode == Constants.OdsCode.None;
            }
        }

        private static void AddSilverIntegrationValidations(JourneysValidator journeysValidator)
        {
            journeysValidator
                .Add(journeys => journeys.SilverIntegrations?.AccountAdmin != null,
                    "journeys.SilverIntegrations.AccountAdmin")
                .Add(journeys => journeys.SilverIntegrations?.CarePlans != null,
                    "journeys.SilverIntegrations.CarePlans")
                .Add(journeys => journeys.SilverIntegrations?.Consultations != null,
                    "journeys.SilverIntegrations.Consultations")
                .Add(journeys => journeys.SilverIntegrations?.ConsultationsAdmin != null,
                    "journeys.SilverIntegrations.ConsultationsAdmin")
                .Add(journeys => journeys.SilverIntegrations?.HealthTrackers != null,
                    "journeys.SilverIntegrations.HealthTrackers")
                .Add(journeys => journeys.SilverIntegrations?.Libraries != null,
                    "journeys.SilverIntegrations.Libraries")
                .Add(journeys => journeys.SilverIntegrations?.Medicines != null,
                    "journeys.SilverIntegrations.Medicines")
                .Add(journeys => journeys.SilverIntegrations?.Messages != null,
                    "journeys.SilverIntegrations.Messages")
                .Add(journeys => journeys.SilverIntegrations?.Participation != null,
                    "journeys.SilverIntegrations.Participation")
                .Add(journeys => journeys.SilverIntegrations?.SecondaryAppointments != null,
                    "journeys.SilverIntegrations.SecondaryAppointments")
                .Add(journeys => journeys.SilverIntegrations?.TestResults != null,
                    "journeys.SilverIntegrations.TestResults");
        }

        private static void AddIm1MessagingValidations(JourneysValidator journeysValidator)
        {
            journeysValidator
                .Add(journeys => journeys.Im1Messaging?.IsEnabled != null,
                    "journeys.im1Messaging.isEnabled")
                .Add(journeys => journeys.Im1Messaging?.CanDeleteMessages != null,
                    "journeys.Im1Messaging.CanDeleteMessages")
                .Add(journeys => journeys.Im1Messaging?.CanUpdateReadStatus != null,
                    "journeys.Im1Messaging.CanUpdateReadStatus")
                .Add(journeys => journeys.Im1Messaging?.RequiresDetailsRequest != null,
                    "journeys.Im1Messaging.RequiresDetailsRequest")
                .Add(journeys => journeys.Im1Messaging?.SendMessageSubject != null,
                    "journeys.Im1Messaging.SendMessageSubject");
        }

        private static void AddHomeScreenValidations(JourneysValidator journeysValidator)
        {
            journeysValidator
                .Add(journeys =>
                {
                    var homeScreen = journeys.HomeScreen;

                    if (homeScreen == null)
                    {
                        return true;
                    }

                    return homeScreen.PublicHealthNotifications?.Any() ?? false;
                }, "journeys.HomeScreen.PublicHealthNotifications");
        }
    }
}
