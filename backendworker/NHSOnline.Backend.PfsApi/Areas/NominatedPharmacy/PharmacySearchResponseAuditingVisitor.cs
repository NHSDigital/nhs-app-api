using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    public class PharmacySearchResponseAuditingVisitor : IPharmacySearchResponseVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<NominatedPharmacyController> _logger;
        
        private const string AuditType = AuditingOperations.SearchNominatedPharmacyAuditTypeResponse;

        public PharmacySearchResponseAuditingVisitor(IAuditor auditor, ILogger<NominatedPharmacyController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(PharmacySearchResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, $"Returning { result.Pharmacies.Count() } pharmacies");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PharmacySearchResult.Success)}");
            }
        }

        public async Task Visit(PharmacySearchResult.InvalidPostcode result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Didn't recognise as valid postcode");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PharmacySearchResult.InvalidPostcode)}");
            }        
        }


        public async Task Visit(PharmacySearchResult.PostcodeResultFailure result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unsuccessful or no postcode search data returned");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PharmacySearchResult.PostcodeResultFailure)}");
            }
        }
        
        public async Task Visit(PharmacySearchResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, " Null or all whitespace Postcode");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PharmacySearchResult.BadRequest)}");
            }
        } 
        
        public async Task Visit(PharmacySearchResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Search for Nhs pharmacies Failed");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PharmacySearchResult.InternalServerError)}");
            }
        } 
        
        public async Task Visit(PharmacySearchResult.ModelValidationError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error, Model State is invalid : bad request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PharmacySearchResult.ModelValidationError)}");
            }
        }
    }
}