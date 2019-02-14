using System;
using NHSOnline.Backend.Worker.Ndop;
using NHSOnline.Backend.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.Areas.Ndop
{
    public class NdopAuditingVisitor : INdopResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<NdopController> _logger;
        
        private const string AuditType = Constants.AuditingTitles.ViewPatientRecordAuditTypeResponse;

        public NdopAuditingVisitor(IAuditor auditor, ILogger<NdopController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(GetNdopResult.SuccessfullyRetrieved result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Ndop Token successfully retrieved");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNdopResult.SuccessfullyRetrieved)}");
            }
        }

        public async Task Visit(GetNdopResult.Unsuccessful result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Unsuccessful");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNdopResult.Unsuccessful)}");
            }
        }
    }
}