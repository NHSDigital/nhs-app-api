using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.NominatedPharmacy.Models;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    public class UpdateNominatedPharmacyResponseAuditingVisitor : IUpdateNominatedPharmacyResponseVisitor<Task>
    {
        private readonly IAuditor _auditor;
         private readonly ILogger<NominatedPharmacyController> _logger;

        private const string AuditType = AuditingOperations.UpdatedNominatedPharmacyResponse;

        public UpdateNominatedPharmacyResponseAuditingVisitor(IAuditor auditor, ILogger<NominatedPharmacyController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(UpdateNominatedPharmacyResponse.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, 
                    $"Successfully updated nominated pharmacy from { result.OldOdsCode } to { result.NewOdsCode }");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(UpdateNominatedPharmacyResponse.Success)}");
            }        
        }

        public async Task Visit(UpdateNominatedPharmacyResponse.GetNominatedPharmacyFailure result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Failed to retrieve Nominated Pharmacy step prior to Updating Nominated Pharmacy"); 
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(UpdateNominatedPharmacyResponse.GetNominatedPharmacyFailure)}");
            }         
        }

        public async Task Visit(UpdateNominatedPharmacyResponse.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, $"Error retrieving nominated pharmacy from Spine - HttpStatusCode { result.StatusCode }");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(UpdateNominatedPharmacyResponse.InternalServerError)}");
            }         
        }

        public async Task Visit(UpdateNominatedPharmacyResponse.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, 
                    $"Error updating nominated pharmacy from { result.OldOdsCode } to { result.NewOdsCode }");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(UpdateNominatedPharmacyResponse.BadGateway)}");
            }         
        }

        public async Task Visit(UpdateNominatedPharmacyResponse.UpdatedButStillOldCode result)
        {
            try
            {
                await _auditor.Audit(AuditType, 
                    $"Nominated pharmacy update of ods code from { result.OldOdsCode } to { result.NewOdsCode } " +
                    "was accepted  but call to get nominated pharmacy still returns the old ods code.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(UpdateNominatedPharmacyResponse.UpdatedButStillOldCode)}");
            }         
        }

        public async Task Visit(UpdateNominatedPharmacyResponse.NominatedPharmacyIsDisabled result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Nominated pharmacy feature is disabled");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(UpdateNominatedPharmacyResponse.NominatedPharmacyIsDisabled)}");
            }
        }

        public async Task Visit(UpdateNominatedPharmacyResponse.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error updating nominated pharamcy : Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(UpdateNominatedPharmacyResponse.BadRequest)}");
            }
        }
    }
}