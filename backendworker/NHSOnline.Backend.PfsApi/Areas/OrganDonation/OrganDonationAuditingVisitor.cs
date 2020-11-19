using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    public class OrganDonationAuditingVisitor : IOrganDonationResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<OrganDonationController> _logger;
        private readonly IMetricLogger _metricLogger;
        private readonly P9UserSession _userSession;

        private const string AuditType = AuditingOperations.GetOrganDonationAuditTypeResponse;

        public OrganDonationAuditingVisitor(IAuditor auditor, ILogger<OrganDonationController> logger,
            IMetricLogger metricLogger, P9UserSession userSession)
        {
            _auditor = auditor;
            _logger = logger;
            _metricLogger = metricLogger;
            _userSession = userSession;
        }

        public async Task Visit(OrganDonationResult.NewRegistration result)
        {
            try
            {
                await _auditor.Audit(AuditType, "A default organ donation registration has been generated");
                await _metricLogger.OrganDonationGetRegistration(new OrganDonationData(_userSession.Key));
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
                await _metricLogger.OrganDonationGetRegistration(new OrganDonationData(_userSession.Key));
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
        
        public async Task Visit(OrganDonationResult.SearchUpstreamError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "There was an upstream error when searching for an organ donation record");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrganDonationResult.SearchUpstreamError)}");
            }
        }
    }
}
