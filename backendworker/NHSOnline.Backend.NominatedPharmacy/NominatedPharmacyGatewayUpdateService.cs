using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyGatewayUpdateService : INominatedPharmacyGatewayUpdateService
    {
        private readonly INominatedPharmacyService _nominatedPharmacyService;
        private readonly IAuditor _auditor;
        private readonly ILogger<NominatedPharmacyGatewayUpdateService> _logger;
        private readonly INominatedPharmacyConfigurationSettings _config;

        public NominatedPharmacyGatewayUpdateService(
            INominatedPharmacyService nominatedPharmacyService,
            IAuditor auditor,
            ILogger<NominatedPharmacyGatewayUpdateService> logger,
            INominatedPharmacyConfigurationSettings config
            )
        {
            _nominatedPharmacyService = nominatedPharmacyService;
            _auditor = auditor;
            _logger = logger;
            _config = config;
        }

        public async Task<StatusCodeResult> UpdateNominatedPharmacy(string nhsNumber, string updatedOdsCode,
            CitizenIdUserSession cidUserSession)
        {
            _logger.LogEnter();
            
            var result = await _nominatedPharmacyService.GetNominatedPharmacy(nhsNumber, cidUserSession);

            if (NominatedPharmacyGetHasError(result, out StatusCodeResult errorResult))
            {
                return errorResult;
            }

            _logger.LogInformation($"Nominated pharmacy retrieved. Updating nominated pharmacy from { result.PharmacyOdsCode } to: { updatedOdsCode }");
            await _auditor.Audit(Constants.AuditingTitles.UpdatedNominatedPharmacy, $"Attempting to update nominated pharmacy from" +
                                                                                            $" { result.PharmacyOdsCode } to: { updatedOdsCode }");

            var nominatedPharmacyUpdate = new NominatedPharmacyUpdate
            {
                NhsNumber = nhsNumber,
                HasExistingNominatedPharmacy = !string.IsNullOrEmpty(result.PharmacyOdsCode),
                UpdatedOdsCode = updatedOdsCode,
                PertinentSerialChangeNumber = result.PertinentSerialChangeNumber,
            };

            var updateNominatedPharmacyResult = await _nominatedPharmacyService.UpdateNominatedPharmacy(nominatedPharmacyUpdate);

            if (!HttpStatusCodeExtensions.IsSuccessStatusCode(updateNominatedPharmacyResult.HttpStatusCode))
            {
                string errorMessage = $"Error updating nominated pharmacy from { result.PharmacyOdsCode } to { updatedOdsCode }";
                _logger.LogError(errorMessage);
                await _auditor.Audit(Constants.AuditingTitles.UpdatedNominatedPharmacy, errorMessage);
                return new StatusCodeResult((int)HttpStatusCode.BadGateway);
            }

            string successMessage = $"Successfully requested requested change of nominated pharmacy from { result.PharmacyOdsCode } to { updatedOdsCode }";
            _logger.LogInformation(successMessage);
            await _auditor.Audit(Constants.AuditingTitles.UpdatedNominatedPharmacy, successMessage);

            // Update to Nominated Pharmacy in Spine is asynchronous. Allow a configurable delay.
            if (_config.ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds > 0)
            {
                await Task.Delay(_config.ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds);
            }

            // Retrieve nominated pharmacy again to confirm it's been updated.
            var confirmNominatedPharmacyUpdatedResult = await _nominatedPharmacyService.GetNominatedPharmacy(nhsNumber, cidUserSession);

            if (!HttpStatusCodeExtensions.IsSuccessStatusCode(result.HttpStatusCode))
            {
                _logger.LogInformation($"Error retrieving nominated pharmacy from Spine - HttpStatusCode { result.HttpStatusCode }");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (!string.Equals(updatedOdsCode, confirmNominatedPharmacyUpdatedResult.PharmacyOdsCode, StringComparison.Ordinal))
            {
                string error = $"Nominated pharmacy update of ods code from { result.PharmacyOdsCode } to { updatedOdsCode } was accepted  " +
                    $"but call to get nominated pharmacy still returns the old ods code.";
                _logger.LogError(error);
                await _auditor.Audit(Constants.AuditingTitles.UpdatedNominatedPharmacy, error);
                return new StatusCodeResult((int)HttpStatusCode.BadGateway);
            }

            await _auditor.Audit(
                Constants.AuditingTitles.UpdatedNominatedPharmacy,
                $"Successfully updated nominated pharmacy from { result.PharmacyOdsCode } to { updatedOdsCode }");

            return new OkResult();
        }

        private bool NominatedPharmacyGetHasError(GetNominatedPharmacyResult result, out StatusCodeResult errorResult)
        {
            if (!HttpStatusCodeExtensions.IsSuccessStatusCode(result.HttpStatusCode))
            {
                _logger.LogInformation($"Error retrieving nominated pharmacy from Spine - HttpStatusCode { result.HttpStatusCode }");
                errorResult = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return true;
            }

            if (string.IsNullOrEmpty(result.PertinentSerialChangeNumber))
            {
                _logger.LogError("Missing pertinentSerialChangeNumber which is required for the update request");
                errorResult = new StatusCodeResult((int)HttpStatusCode.BadGateway);
                return true;
            }
            
            if (!result.HaveAllChecksPassed)
            {
                _logger.LogError("Invalid pharmacy type, combination or family name / dob mismatch");
                errorResult = new StatusCodeResult((int)HttpStatusCode.BadGateway);
                return true;
            }

            errorResult = null;
            return false;
        }

    }
}