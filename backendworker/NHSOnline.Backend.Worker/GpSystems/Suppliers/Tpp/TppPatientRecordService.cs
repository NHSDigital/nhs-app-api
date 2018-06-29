using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppPatientRecordService : IPatientRecordService
    {
        private readonly ILogger _logger;
        private readonly ITppClient _tppClient;
        private readonly ITppMyRecordMapper _tppMyRecordMapper;
        
        public TppPatientRecordService(ILoggerFactory loggerFactory, ITppClient tppClient, ITppMyRecordMapper tppMyRecordMapper)
        {
            _tppClient = tppClient;
            _tppMyRecordMapper = tppMyRecordMapper;
            _logger = loggerFactory.CreateLogger<TppPatientRecordService>();
        }
        
        public async Task<GetMyRecordResult> Get(UserSession userSession)
        {
            var tppUserSession = (TppUserSession) userSession;

            try
            {
                var request = new ViewPatientOverview
                {
                    PatientId = tppUserSession.PatientId,
                    OnlineUserId = tppUserSession.OnlineUserId,
                    UnitId = tppUserSession.UnitId,
                };
                
                var patientOverviewTask = _tppClient.PatientOverviewPost(request, tppUserSession.Suid);
                await Task.WhenAll(patientOverviewTask);
                
                var patientOverviewItems = new GetPatientOverviewTaskChecker(_logger).Check(patientOverviewTask);
                var allergies = patientOverviewItems.Item1;
                var medications = patientOverviewItems.Item2;
                var immunisations = new Immunisations();
                var testResults = new TestResults();
                var problems = new Problems();
                var myRecordResponse = _tppMyRecordMapper.Map(allergies, medications, immunisations, testResults, problems);
                
                _logger.LogInformation("MyRecordResponse: " + myRecordResponse);

                return new GetMyRecordResult.SuccessfullyRetrieved(myRecordResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving my record");
                return new GetMyRecordResult.Unsuccessful();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "My record retrieval return null body");
                return new GetMyRecordResult.SupplierBadData();
            }
        }
    }
}