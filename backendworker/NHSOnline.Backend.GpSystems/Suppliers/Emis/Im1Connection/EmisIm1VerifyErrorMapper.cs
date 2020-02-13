using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Im1Connection
{
    public static class EmisIm1VerifyErrorMapper 
    {
        private static KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.InternalCode>
            KeyAndMessageToError =>
            new KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.InternalCode>()
                .Add("4031030", "Patient facing services API v2 is not enabled at this practice",
                    Im1ConnectionErrorCodes.InternalCode.PatientFacingServicesApiv2IsNotEnabledAtThisPractice);

        public static Im1ConnectionVerifyResult Map<T>(EmisClient.EmisApiObjectResponse<T> response,
            ILogger<EmisIm1ConnectionService> logger)
        {
            var mappedValue = EmisErrorMapper.Map(logger, response, KeyAndMessageToError);
            return mappedValue != null ? new Im1ConnectionVerifyResult.ErrorCase(mappedValue.Value) :
                (Im1ConnectionVerifyResult) new Im1ConnectionVerifyResult.UnmappedErrorWithStatusCode();
        }
    }
}
