using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Demographics;

namespace NHSOnline.Backend.PfsApi.Areas.Demographics
{
    public class DemographicsAuditingVisitor : IDemographicsResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<DemographicsController> _logger;
        private const string AuditType = AuditingOperations.GetDemographicsAuditTypeResponse;

        public DemographicsAuditingVisitor(IAuditor auditor, ILogger<DemographicsController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(DemographicsResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error viewing Demographics: patient does not have access to data");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DemographicsResult.Forbidden)}");
            }
        }

        public async Task Visit(DemographicsResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Demographics successfully viewed");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DemographicsResult.Success)}");
            }
        }

        public async Task Visit(DemographicsResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error viewing Demographics: bad gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DemographicsResult.BadGateway)}");
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
