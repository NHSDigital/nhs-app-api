using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    public class CreateLinkageKeysService : ICreateLinkageKeysService
    {
        private readonly ILogger<CreateLinkageKeysService> _logger;
        private readonly IAuditor _auditor;
        private readonly IMinimumAgeValidator _minimumAgeValidator;
        private readonly ConfigurationSettings _settings;

        public CreateLinkageKeysService(
            ILogger<CreateLinkageKeysService> logger,
            IAuditor auditor,
            IMinimumAgeValidator minimumAgeValidator,
            ConfigurationSettings settings)
        {
            _logger = logger;
            _auditor = auditor;
            _minimumAgeValidator = minimumAgeValidator;
            _settings = settings;
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest request, IGpSystem gpSystem)
        {
            try
            {
                if (!HasValidDateOfBirthForLinkage(request.DateOfBirth))
                {
                    _logger.LogWarning("Linkage details request unsuccessful - patient non competent or under 16.");

                    return new LinkageResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode.UnderMinimumAgeOrNonCompetent);
                }

                var linkageService = gpSystem.GetLinkageService();

                await _auditor.PreOperationAuditRegistrationEvent(request.NhsNumber, gpSystem.Supplier,
                    AuditingOperations.CreateLinkageKeyAuditTypeRequest, "Attempting to create linkage key.");

                return await linkageService.CreateLinkageKey(request);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private bool HasValidDateOfBirthForLinkage(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
            {
                _logger.LogError("Missing date of birth");
                return false;
            }

            _logger.LogWarning($"Minumum age {_settings.MinimumLinkageAge}");
            if (!_minimumAgeValidator.IsValid(dateOfBirth.Value, _settings.MinimumLinkageAge))
            {
                _logger.LogWarning("Failed to meet the minimum linkage age requirement.");
                return false;
            }

            return true;
        }
    }
}
