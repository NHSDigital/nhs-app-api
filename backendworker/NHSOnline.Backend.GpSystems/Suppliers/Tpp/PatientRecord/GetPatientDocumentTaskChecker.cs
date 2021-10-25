using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface IGetPatientDocumentTaskChecker
    {
        PatientDocument CheckForViewing(TppApiObjectResponse<RequestBinaryDataReply> task);
        FileContentResult CheckForDownload(TppApiObjectResponse<RequestBinaryDataReply> response, string documentName);
    }

    public class GetPatientDocumentTaskChecker : IGetPatientDocumentTaskChecker
    {
        private readonly ITppDocumentMapper _documentMapper;
        private readonly ILogger<IGetPatientDocumentTaskChecker> _logger;

        public GetPatientDocumentTaskChecker(ITppDocumentMapper dcrDocumentsMapper, ILogger<GetPatientDocumentTaskChecker> logger)
        {
            _documentMapper = dcrDocumentsMapper;
            _logger = logger;
        }

        public PatientDocument CheckForViewing(TppApiObjectResponse<RequestBinaryDataReply> task)
        {
            try
            {
                _logger.LogEnter();

                var defaultResult = new PatientDocument
                {
                    HasErrored = true
                };

                if (task is null)
                {
                    return defaultResult;
                }

                if (task.HasSuccessResponse)
                {
                    return _documentMapper.Map(task.Body);
                }

                if (task.HasErrorWithCode(Constants.TppErrorCodes.FileTooLarge))
                {
                    _logger.LogError("File is too large, setting IsViewable and IsDownloadable to false");
                    return new PatientDocument
                    {
                        IsViewable = false,
                        IsDownloadable = false
                    };

                }

                if (task.HasErrorWithCode(Constants.TppErrorCodes.FileStillUploading))
                {
                    _logger.LogError("File is still uploading, setting IsViewable and IsDownloadable to false");
                    return new PatientDocument
                    {
                        IsViewable = false,
                        IsDownloadable = false
                    };
                }

                _logger.LogError(task.HasForbiddenResponse
                    ? "User does not have access to request binary data"
                    : $"Unsuccessful request retrieving binary data. Status code: {(int) task.StatusCode}");

                return defaultResult;
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public FileContentResult CheckForDownload(
            TppApiObjectResponse<RequestBinaryDataReply> response,
            string documentName)
        {
            try
            {
                _logger.LogEnter();

                if (response is null)
                {
                    throw new ArgumentNullException(nameof(response));
                }

                if (response.HasSuccessResponse)
                {
                    return _documentMapper.MapForDownload(response.Body, documentName);
                }

                if (response.HasErrorWithCode(Constants.TppErrorCodes.FileTooLarge))
                {
                    _logger.LogError("File is too large");

                }
                else if (response.HasErrorWithCode(Constants.TppErrorCodes.FileStillUploading))
                {
                    _logger.LogError("File is still uploading");
                }
                else
                {
                    _logger.LogError(response.HasForbiddenResponse
                        ? "User does not have access to request binary data"
                        : $"Unsuccessful request retrieving binary data. Status code: {(int) response.StatusCode}");
                }

                return null;
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}