using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Im1Connection
{
    public static class VisionIm1RegisterErrorMapper
    {
        private static readonly KeyAndMessageToEnumMapper<InternalCode> KeyAndMessageToError =
            new KeyAndMessageToEnumMapper<InternalCode>()
                .Add("200-100", "Connection to external service failed",
                    InternalCode.ConnectionToServiceFailed)
                .Add("200-100", "Unknown Error", InternalCode.UnknownError)
                .AddKeyToEnum("200-31", InternalCode.InvalidLinkageDetails)
                .AddKeyToEnum("200-34", InternalCode.UserAccountIsInactiveOrArchived)
                .AddKeyToEnum("400V4205", InternalCode.InvalidNhsNumber)
                .AddKeyToEnum("404V4205", InternalCode.InvalidNhsNumber)
                .AddKeyToEnum("200-33", InternalCode.NoUserFoundForLinkageDetails)
                .AddKeyToEnum("400-33", InternalCode.NoUserFoundForLinkageDetails)
                .AddKeyToEnum("404-33", InternalCode.NoUserFoundForLinkageDetails)
                .AddKeyToEnum("200-2", InternalCode.UserAlreadyLinked)
                .AddKeyToEnum("200-19", InternalCode.UserAccountDisabled)
                .AddKeyToEnum("200-15", InternalCode.UserAccountDisabled);

        public static Im1ConnectionRegisterResult Map(
            VisionPFSClient.VisionApiObjectResponse<ServiceContentRegisterResponse> response, ILogger<VisionIm1ConnectionService> logger)
        {
            var mappedValue = VisionErrorMapper.Map(logger, response, KeyAndMessageToError);

            return mappedValue != null
                ? new Im1ConnectionRegisterResult.ErrorCase(mappedValue.Value)
                : (Im1ConnectionRegisterResult) new Im1ConnectionRegisterResult.UnmappedErrorWithStatusCode();
        }
    }
}
