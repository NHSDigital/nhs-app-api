using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    public class MicrotestPatientRecordService : IPatientRecordService
    {
        private readonly ILogger<MicrotestPatientRecordService> _logger;

        public MicrotestPatientRecordService(
            ILogger<MicrotestPatientRecordService> logger
        )
        {
            _logger = logger;
        }
        
        public async Task<GetMyRecordResult> GetMyRecord(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            var myRecordResponse = new MyRecordResponse
            {
                HasDetailedRecordAccess = true
            };
            
            _logger.LogInformation("Returning empty MyRecordResponse for Microtest Patient Record Service");
            return await Task.FromResult(new GetMyRecordResult.Success(myRecordResponse));
        }

        public Task<GetDetailedTestResult> GetDetailedTestResult(GpUserSession gpUserSession, string testResultId)
        {
            throw new NotImplementedException();
        }
    }
}