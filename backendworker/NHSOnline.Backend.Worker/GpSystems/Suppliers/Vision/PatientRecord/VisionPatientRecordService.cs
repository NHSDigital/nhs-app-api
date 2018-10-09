using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionPatientRecordService : IPatientRecordService
    {
        private readonly ILogger<VisionPatientRecordService>  _logger;
        private readonly IVisionClient _visionClient;
        private readonly IVisionMyRecordMapper _visionMyRecordMapper;

        public VisionPatientRecordService(ILogger<VisionPatientRecordService> logger, IVisionClient visionClient, IVisionMyRecordMapper visionMyRecordMapper)
        {
            _logger = logger;
            _visionClient = visionClient;
            _visionMyRecordMapper = visionMyRecordMapper;
        }

        public async Task<GetMyRecordResult> GetMyRecord(UserSession userSession)
        {
            return await Task.FromResult<GetMyRecordResult>(new GetMyRecordResult.SuccessfullyRetrieved(new MyRecordResponse()));
        }

        public Task<GetDetailedTestResult> GetDetailedTestResult(UserSession userSession, string testResultId)
        {
            throw new NotImplementedException();
        }
    }
}