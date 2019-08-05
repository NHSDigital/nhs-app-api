using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.OrganDonation;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    public class OrganDonationReferenceDataAuditingVisitor : IOrganDonationReferenceDataResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<OrganDonationReferenceDataController> _logger;
        
        private const string AuditType = AuditingOperations.GetOrganDonationReferenceDataAuditTypeResponse;

        public OrganDonationReferenceDataAuditingVisitor(IAuditor auditor, ILogger<OrganDonationReferenceDataController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(OrganDonationReferenceDataResult.SuccessfullyRetrieved result)
        {
            try
            {
                await _auditor.Audit(AuditType, "The organ donation reference data has been retrieved successfully");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationReferenceDataResult.SuccessfullyRetrieved)}");
            }
        }
        
        public async Task Visit(OrganDonationReferenceDataResult.SystemError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "There was an issue getting the organ donation reference data");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationReferenceDataResult.SystemError)}");
            }
        }

        public async Task Visit(OrganDonationReferenceDataResult.UpstreamError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "There was an upstream error when getting the organ donation reference data");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationReferenceDataResult.UpstreamError)}");
            }
        }

        public async Task Visit(OrganDonationReferenceDataResult.Timeout result)
        {
            try
            {
                await _auditor.Audit(AuditType, "The organ donation reference data system took too long to respond");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationReferenceDataResult.Timeout)}");
            }
        }
    }
}
