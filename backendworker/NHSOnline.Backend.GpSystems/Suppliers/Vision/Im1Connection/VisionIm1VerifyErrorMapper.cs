using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Im1Connection
{
    public static class VisionIm1VerifyErrorMapper
    {
        private static readonly KeyAndMessageToEnumMapper<InternalCode> KeyAndMessageToError =
            new KeyAndMessageToEnumMapper<InternalCode>()
                .AddKeyToEnum("200-15", InternalCode.UserRecordUnavailable)
                .AddKeyToEnum("200-100", InternalCode.ConnectionToServiceFailed)
                .AddKeyToEnum("200-30", InternalCode.InvalidLinkageDetails)
                // THESE RESPONSES DO NOT HAVE ERROR CODES JUST A FAULT CODE 
                .AddKeyToEnum("200INVALID_REQUEST", InternalCode.InvalidRequest)
                .AddKeyToEnum("200InvalidSecurity", InternalCode.InvalidSecurity);

        public static Im1ConnectionVerifyResult Map(
            VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse> response, ILogger<VisionIm1ConnectionService> logger)
        {
            var mappedValue = VisionErrorMapper.Map(logger, response, KeyAndMessageToError);

            return mappedValue != null
                ? new Im1ConnectionVerifyResult.ErrorCase(mappedValue.Value)
                : (Im1ConnectionVerifyResult) new Im1ConnectionVerifyResult.UnmappedErrorWithStatusCode();
        }
    }
}
