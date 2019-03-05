using System;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    public class OrganDonationAuditingVisitor : IOrganDonationResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<OrganDonationController> _logger;
        
        private const string AuditType = Constants.AuditingTitles.GetOrganDonationAuditTypeResponse;

        public OrganDonationAuditingVisitor(IAuditor auditor, ILogger<OrganDonationController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(OrganDonationResult.NewRegistration result)
        {
            try
            {
                await _auditor.Audit(AuditType, "A default organ donation registration has been generated");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationResult.NewRegistration)}");
            }
        }

        public async Task Visit(OrganDonationResult.ExistingRegistration result)
        {
            try
            {
                await _auditor.Audit(AuditType, "An existing organ donation registration been found");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationResult.ExistingRegistration)}");
            }
        }

        public async Task Visit(OrganDonationResult.DemographicsRetrievalFailed result)
        {
            try
            {
                await _auditor.Audit(AuditType, "There was an issue retrieving the demographics record");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationResult.DemographicsRetrievalFailed)}");
            }
        }

        public async Task Visit(OrganDonationResult.DemographicsForbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Access to demographics was forbidden");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationResult.DemographicsForbidden)}");
            }
        }

        public async Task Visit(OrganDonationResult.DemographicsInternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error received from demographics");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationResult.DemographicsInternalServerError)}");
            }
        }

        public async Task Visit(OrganDonationResult.DemographicsBadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "There was an issue retrieving the demographics record");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationResult.DemographicsBadGateway)}");
            }

        }

        public async Task Visit(OrganDonationResult.SearchSystemUnavailable result)
        {
            try
            {
                await _auditor.Audit(AuditType, "The organ donation system is unavailable");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationResult.SearchSystemUnavailable)}");
            }
        }

        public async Task Visit(OrganDonationResult.BadSearchRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "The search request is invalid");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationResult.BadSearchRequest)}");
            }
        }

        public async Task Visit(OrganDonationResult.SearchTimeout result)
        {
            try
            {
                await _auditor.Audit(AuditType, "The organ donation system took too long to respond");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationResult.SearchTimeout)}");
            }
        }

        public async Task Visit(OrganDonationResult.SearchError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "There was an issue searching for an organ donation record");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationResult.SearchError)}");
            }
        }
    }
}
