using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    public class DetailedTestResultAuditingVisitor : IDetailedTestResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<DetailedTestResultController> _logger;
        private const string AuditType = AuditingOperations.GetTestResultAuditTypeResponse;

        public DetailedTestResultAuditingVisitor(IAuditor auditor, ILogger<DetailedTestResultController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(GetDetailedTestResult.Success result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Test result successfully viewed");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetDetailedTestResult.Success)}");
            }
        }

        public async Task Visit(GetDetailedTestResult.BadGateway result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error viewing test result: bad gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetDetailedTestResult.BadGateway)}");
            }
        }
    }
}
