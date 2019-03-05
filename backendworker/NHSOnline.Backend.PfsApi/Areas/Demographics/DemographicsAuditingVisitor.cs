using System;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Demographics
{
    public class DemographicsAuditingVisitor : IDemographicsResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<DemographicsController> _logger;
        private const string AuditType = Constants.AuditingTitles.GetDemographicsAuditTypeResponse;

        public DemographicsAuditingVisitor(IAuditor auditor, ILogger<DemographicsController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(DemographicsResult.UserHasNoAccess result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error viewing Demographics: patient does not have access to data");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DemographicsResult.UserHasNoAccess)}");
            }
        }

        public async Task Visit(DemographicsResult.SuccessfullyRetrieved result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Demographics successfully viewed");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DemographicsResult.SuccessfullyRetrieved)}");
            }
        }

        public async Task Visit(DemographicsResult.SupplierSystemUnavailable supplierSystemUnavailable)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error viewing Demographics: supplier system unavailable");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DemographicsResult.SupplierSystemUnavailable)}");
            }

        }

        public async Task Visit(DemographicsResult.Unsuccessful result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error viewing Demographics: unsuccessful");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DemographicsResult.Unsuccessful)}");
            }
        }

        public async Task Visit(DemographicsResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error viewing Demographics: internal server error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DemographicsResult.InternalServerError)}");
            }
        }
    }
}
