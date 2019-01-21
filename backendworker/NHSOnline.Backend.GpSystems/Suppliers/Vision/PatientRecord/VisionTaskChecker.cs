using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionTaskChecker<T> where T: IPatientDataModel, new()
    {
        private readonly IVisionMapper<T> _visionMapper;
        private readonly ILogger _logger;
        private readonly VisionMapperType _mapperType;
        
        public VisionTaskChecker(ILogger logger, IVisionMapper<T> visionMapper, VisionMapperType mapperType)
        {
            _logger = logger;
            _visionMapper = visionMapper;
            _mapperType = mapperType;
        }

        public T Check(VisionPFSClient.VisionApiObjectResponse<VisionPatientDataResponse> result)
        {
            _logger.LogInformation($"Checking Vision {(_mapperType)}");
            
            if (!result.HasErrorResponse) 
                return _visionMapper.Map(result.Body);
            
            if (result.IsAccessDeniedError)
            {
                _logger.LogWarning("User does not have access to their patient record");
                return new T
                {
                    HasAccess = false
                };
            }

            _logger.LogError($"Unsuccessful request retrieving {(_mapperType)} information for Vision. Status code: {(int)result.StatusCode}");
            return new T
            {
                HasErrored = true
            };
        }
    }
}