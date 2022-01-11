using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    public class HistoricTestResultsAuditingVisitor : IHistoricTestResultsVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<HistoricTestResultsController> _logger;
        private readonly int _year;

        private const string AuditType = AuditingOperations.GetHistoricTestResultsAuditTypeResponse;

        public HistoricTestResultsAuditingVisitor(IAuditor auditor, ILogger<HistoricTestResultsController> logger, int year)
        {
            _auditor = auditor;
            _logger = logger;
            _year = year;
        }

        public async Task Visit(GetHistoricTestResultsResult.Success result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, $"Successfully retrieved Historic Test Results for {_year}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetHistoricTestResultsResult.Success)}");
            }
        }

        public async Task Visit(GetHistoricTestResultsResult.BadGateway result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, $"Error trying to retrieve Historic Test Results for {_year}: bad gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetHistoricTestResultsResult.BadGateway)}");
            }
        }
    }
}
