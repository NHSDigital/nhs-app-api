using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
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

        public async Task<UpdateNominatedPharmacyResponse> UpdateNominatedPharmacy(string nhsNumber, string updatedOdsCode,
            CitizenIdUserSession cidUserSession)
        {
            _logger.LogEnter();

            var getNominatedPharmacyResult = await _nominatedPharmacyService.GetNominatedPharmacy(nhsNumber, cidUserSession);
            var result = getNominatedPharmacyResult.GetNominatedPharmacyResponse;

            if (NominatedPharmacyGetHasError(result, out StatusCodeResult errorResult))
            {
                return new UpdateNominatedPharmacyResponse.GetNominatedPharmacyFailure(errorResult);
            }

            _logger.LogInformation($"Nominated pharmacy retrieved. Updating nominated pharmacy from { result.PharmacyOdsCode } to: { updatedOdsCode }");

            var nominatedPharmacyUpdate = new NominatedPharmacyUpdate
            {
                NhsNumber = nhsNumber,
                HasExistingNominatedPharmacy = !string.IsNullOrEmpty(result.PharmacyOdsCode),
                UpdatedOdsCode = updatedOdsCode,
                PertinentSerialChangeNumber = result.PertinentSerialChangeNumber,
                ObjectId = result.ObjectId,
            };

            var updateNominatedPharmacyResult = await _nominatedPharmacyService.UpdateNominatedPharmacy(nominatedPharmacyUpdate);

            if (!updateNominatedPharmacyResult.HttpStatusCode.IsSuccessStatusCode())
            {
                _logger.LogError($"Error updating nominated pharmacy from { result.PharmacyOdsCode } to { updatedOdsCode }");
                return new UpdateNominatedPharmacyResponse.BadGateway(result.PharmacyOdsCode, updatedOdsCode);
            }

            _logger.LogInformation($"Successfully requested requested change of nominated pharmacy from { result.PharmacyOdsCode } to { updatedOdsCode }");

            // Update to Nominated Pharmacy in Spine is asynchronous. Allow a configurable delay.
            if (_config.ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds > 0)
            {
                await Task.Delay(_config.ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds);
            }

            // Retrieve nominated pharmacy again to confirm it's been updated.
            var confirmNominatedPharmacyUpdatedResult = await _nominatedPharmacyService.GetNominatedPharmacy(nhsNumber, cidUserSession);
            var confirmUpdateResponse = confirmNominatedPharmacyUpdatedResult.GetNominatedPharmacyResponse;

            if (!result.HttpStatusCode.IsSuccessStatusCode())
            {
                _logger.LogInformation($"Error retrieving nominated pharmacy from Spine - HttpStatusCode { result.HttpStatusCode }");
                return new UpdateNominatedPharmacyResponse.InternalServerError(result.HttpStatusCode);
            }

            if (!string.Equals(updatedOdsCode, confirmUpdateResponse.PharmacyOdsCode, StringComparison.Ordinal))
            {
                _logger.LogError($"Nominated pharmacy update of ods code from { result.PharmacyOdsCode } to { updatedOdsCode } was accepted  " +
                                 "but call to get nominated pharmacy still returns the old ods code.");

                return new UpdateNominatedPharmacyResponse.UpdatedButStillOldCode(result.PharmacyOdsCode, updatedOdsCode);
            }

            _logger.LogInformation($"Successfully updated nominated pharmacy from { result.PharmacyOdsCode } to { updatedOdsCode }");
            return new UpdateNominatedPharmacyResponse.Success(result.PharmacyOdsCode, updatedOdsCode);
        }

        private bool NominatedPharmacyGetHasError(GetNominatedPharmacyResponse response, out StatusCodeResult errorResult)
        {
            if (!response.HttpStatusCode.IsSuccessStatusCode())
            {
                _logger.LogInformation($"Error retrieving nominated pharmacy from Spine - HttpStatusCode { response.HttpStatusCode }");
                errorResult = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return true;
            }

            if (string.IsNullOrEmpty(response.PertinentSerialChangeNumber))
            {
                _logger.LogError("Missing pertinentSerialChangeNumber which is required for the update request");
                errorResult = new StatusCodeResult((int)HttpStatusCode.BadGateway);
                return true;
            }

            if (!response.HaveAllChecksPassed)
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