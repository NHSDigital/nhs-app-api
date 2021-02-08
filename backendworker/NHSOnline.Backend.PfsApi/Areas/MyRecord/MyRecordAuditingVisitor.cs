using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    public class MyRecordAuditingVisitor : IMyRecordResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<MyRecordController> _logger;
        private readonly IMetricLogger _metricLogger;
        private readonly P9UserSession _userSession;

        private const string AuditType = AuditingOperations.ViewPatientRecordAuditTypeResponse;

        public MyRecordAuditingVisitor(IAuditor auditor,
            ILogger<MyRecordController> logger,
            IMetricLogger metricLogger,
            P9UserSession userSession)
        {
            _auditor = auditor;
            _logger = logger;
            _metricLogger = metricLogger;
            _userSession = userSession;
        }

        public async Task Visit(GetMyRecordResult.Success result)
        {
            try
            {
                var hasSummaryRecordAccess = result.Response.HasSummaryRecordAccess;
                var hasDetailedRecordAccess = result.Response.HasDetailedRecordAccess;

                await _metricLogger.MedicalRecordView(new MedicalRecordData(_userSession.Key, hasSummaryRecordAccess, hasDetailedRecordAccess));

                await _auditor.PostOperationAudit(AuditType,
                    $"Patient record successfully retrieved. {nameof(hasSummaryRecordAccess)}={hasSummaryRecordAccess}," +
                    $" {nameof(hasDetailedRecordAccess)}={hasDetailedRecordAccess}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.Success)}");
            }
        }

        public async Task Visit(GetMyRecordResult.BadGateway result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error: Unsuccessful");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.BadGateway)}");
            }
        }

        public async Task Visit(GetMyRecordResult.InternalServerError result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error: Internal server error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.InternalServerError)}");
            }
        }
    }
}
