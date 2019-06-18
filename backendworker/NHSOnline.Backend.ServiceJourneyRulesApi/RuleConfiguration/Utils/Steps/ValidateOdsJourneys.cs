using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
                if (EnumHelper.HasValue(journeys.Appointments?.Provider) &&
                    EnumHelper.HasValue(journeys.Prescriptions?.Provider) &&
                    EnumHelper.HasValue(journeys.MedicalRecord?.Provider))
                {
                    _logger.LogDebug($"Validation successful for '{odsCode}'");
                    continue;
                }

                _logger.LogError($"Not all journey types have been defined for '{odsCode}'.\n" +
                                 $"\tPrescription: {journeys.Prescriptions?.Provider}\n" +
                                 $"\tMedicalRecord: {journeys.MedicalRecord?.Provider}\n" +
                                 $"\tAppointments: {journeys.Appointments?.Provider}");
                isValid = false;
            }

            return isValid;
        }
    }
}