using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.VisionClient;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionPatientRecordService : IPatientRecordService
    {
        private readonly ILogger _logger;
        private readonly IVisionClient _visionClient;
        private readonly IVisionMyRecordMapper _visionMyRecordMapper;

        public VisionPatientRecordService(ILogger logger, IVisionClient visionClient, IVisionMyRecordMapper visionMyRecordMapper)
        {
            _logger = logger;
            _visionClient = visionClient;
            _visionMyRecordMapper = visionMyRecordMapper;
        }

        public async Task<GetMyRecordResult> Get(UserSession userSession)
        {
            var visionUserSession = (VisionUserSession) userSession;

            try
            {
                var visionConnectionToken = new VisionConnectionToken
                {
                    RosuAccountId = visionUserSession.RosuAccountId,
                    ApiKey = visionUserSession.ApiKey
                };

                var response = await _visionClient.GetConfiguration(visionConnectionToken, visionUserSession.OdsCode);

                if (!response.HasErrorResponse)
                {
                    var myRecordResponse = _visionMyRecordMapper.Map(response.Body);

                    return new GetMyRecordResult.SuccessfullyRetrieved(myRecordResponse);
                }

                return GetCorrectErrorResult(response);
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

        private GetMyRecordResult GetCorrectErrorResult<T>(VisionApiObjectResponse<T> response)
        {
            if (response.IsInvalidRequestError)
            {
                return new GetMyRecordResult.InvalidRequest();
            }

            if (response.IsInvalidUserCredentialsError)
            {
                return new GetMyRecordResult.InvalidUserCredentials();
            }

            if (response.IsInvalidSecurtyHeaderError)
            {
                return new GetMyRecordResult.ErrorProcessingSecurityHeader();
            }

            if (response.IsUnknownError)
            {
                return new GetMyRecordResult.UnknownError();
            }

            return new GetMyRecordResult.Unsuccessful();
        }
    }
}