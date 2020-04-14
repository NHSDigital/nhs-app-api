using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public interface IGetPatientDocumentTaskChecker
    {
        PatientDocument CheckForViewing(
            EmisClient.EmisApiObjectResponse<IndividualDocument> response,
            string documentType,
            string documentName);

        FileContentResult CheckForDownload(
            EmisClient.EmisApiObjectResponse<IndividualDocument> response,
            string documentType,
            string documentName);
    }
    public class GetPatientDocumentTaskChecker: IGetPatientDocumentTaskChecker
    {
        private readonly ILogger<IGetPatientDocumentTaskChecker> _logger;
        private readonly IEmisPatientDocumentMapper _mapper;
        
        public GetPatientDocumentTaskChecker(ILogger<IGetPatientDocumentTaskChecker> logger, IEmisPatientDocumentMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }
        
        public PatientDocument CheckForViewing(
            EmisClient.EmisApiObjectResponse<IndividualDocument> response,
            string documentType,
            string documentName)
        {
            try
            {
                _logger.LogEnter();

                return Check(response, documentType, documentName, _mapper.Map);
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        public FileContentResult CheckForDownload(
            EmisClient.EmisApiObjectResponse<IndividualDocument> response,
            string documentType,
            string documentName)
        {
            try
            {
                _logger.LogEnter();

                return Check(response, documentType, documentName, _mapper.MapForDownload);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private T Check<T>(
            EmisClient.EmisApiObjectResponse<IndividualDocument> response,
            string documentType,
            string documentName,
            Func<IndividualDocument, string, string, T> mappingFunction)
        {
            if (response.HasSuccessResponse)
            {
                return mappingFunction(response.Body, documentType, documentName);
            }

            var statusCode = (int) response.StatusCode;

            _logger.LogError(
                response.HasBadRequestResponse
                    ? $"Patient document retrieval returned Bad Request. The document guid may not be valid. Status code: {statusCode}"
                    : $"Unsuccessful request retrieving patient document. Status code: {statusCode}");

            return default;
        }
    }
}
