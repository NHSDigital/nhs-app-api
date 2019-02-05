using System;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    public class DetailedTestResultAuditingVisitor : IDetailedTestResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<DetailedTestResultController> _logger;
        
        private const string AuditType = Constants.AuditingTitles.GetTestResultAuditTypeResponse;
        
        public DetailedTestResultAuditingVisitor(IAuditor auditor, ILogger<DetailedTestResultController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(GetDetailedTestResult.SuccessfullyRetrieved result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Test result successfully viewed");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetDetailedTestResult.SuccessfullyRetrieved)}");
            }
        }

        public async Task Visit(GetDetailedTestResult.SupplierBadData result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error viewing test result: supplier bad data");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetDetailedTestResult.SupplierBadData)}");
            }
        }

        public async Task Visit(GetDetailedTestResult.Unsuccessful result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error viewing test result: unsuccessful");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetDetailedTestResult.Unsuccessful)}");
            }
        }

    }
}