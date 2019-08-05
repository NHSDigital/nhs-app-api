using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Ndop;

namespace NHSOnline.Backend.PfsApi.Areas.Ndop
{
    public class NdopAuditingVisitor : INdopResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<NdopController> _logger;
        
        private const string AuditType = AuditingOperations.ViewPatientRecordAuditTypeResponse;

        public NdopAuditingVisitor(IAuditor auditor, ILogger<NdopController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(GetNdopResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Ndop Token successfully retrieved");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNdopResult.Success)}");
            }
        }

        public async Task Visit(GetNdopResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Unsuccessful");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNdopResult.InternalServerError)}");
            }
        }
    }
}