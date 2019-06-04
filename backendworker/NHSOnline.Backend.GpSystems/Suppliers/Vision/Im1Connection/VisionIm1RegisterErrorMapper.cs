using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Im1Connection
{
    public class VisionIm1RegisterErrorMapper : Im1ConnectionErrorMapper
    {
        protected override Code UnknownError =>
            Code.UnknownError;

        private readonly KeyAndMessageToEnumMapper<Code> _keyAndMessageToError =
            new KeyAndMessageToEnumMapper<Code>()
                .Add("200-100", "Connection to external service failed",
                    Code.ConnectionToServiceFailed)
                .AddKeyToEnum("200-31", Code.InvalidLinkageDetails)
                .AddKeyToEnum("200-34", Code.UserAccountIsInactiveOrArchived)
                .AddKeyToEnum("400V4205", Code.InvalidNhsNumber)
                .AddKeyToEnum("404V4205", Code.InvalidNhsNumber)
                .AddKeyToEnum("200-33", Code.NoUserFoundForLinkageDetails)
                .AddKeyToEnum("400-33", Code.NoUserFoundForLinkageDetails)
                .AddKeyToEnum("404-33", Code.NoUserFoundForLinkageDetails)
                .AddKeyToEnum("200-2", Code.UserAlreadyLinked)
                .AddKeyToEnum("200-19", Code.UserAccountDisabled)
                .AddKeyToEnum("200-15", Code.UserAccountDisabled);



        public Im1ConnectionRegisterResult Map(
            VisionPFSClient.VisionApiObjectResponse<ServiceContentRegisterResponse> response, ILogger<VisionIm1ConnectionService> logger)
        {
            logger.LogVisionErrorResponse(response);
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var statusCode = (int) response.StatusCode;
            var visionErrorCode = response.ErrorCode;
            var visionErrorMessage = response.ErrorMessage;
            var key = $"{statusCode}{visionErrorCode}";

            var mappedValue = _keyAndMessageToError.Map(logger, key, visionErrorMessage);

            return mappedValue != null
                ? new Im1ConnectionRegisterResult.ErrorCase(mappedValue.Value)
                : MapUnknownError(response.StatusCode);
        }
    }
}
