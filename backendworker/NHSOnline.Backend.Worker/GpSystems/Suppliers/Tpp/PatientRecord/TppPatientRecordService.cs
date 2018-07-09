using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
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
                
                var patientOverviewTask = _tppClient.PatientOverviewPost(tppUserSession);                
                var patientRecordTask = _tppClient.RequestPatientRecordPost(tppUserSession);
                
                await Task.WhenAll(patientOverviewTask, patientRecordTask);
                
                var patientOverviewItems = new GetPatientOverviewTaskChecker(_logger).Check(patientOverviewTask);
                var dcrEvents = new GetPatientDcrEvents(_logger).Check(patientRecordTask);
                
                var allergies = patientOverviewItems.Item1;
                var medications = patientOverviewItems.Item2;
                
                var myRecordResponse = _tppMyRecordMapper.Map(allergies, medications, dcrEvents);
                myRecordResponse.Supplier = userSession.Supplier.ToString().ToUpper();
                
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