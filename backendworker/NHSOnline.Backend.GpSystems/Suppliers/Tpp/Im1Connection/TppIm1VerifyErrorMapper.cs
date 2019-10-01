using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection
{
    public static class TppIm1VerifyErrorMapper
    {
        private static Dictionary<string, InternalCode> KeyToEnumMapper =>
            new Dictionary<string, InternalCode>
            {
                { "2006", InternalCode.ProblemLoggingIn },
                { "2009", InternalCode.ProblemLoggingIn },
            };

        public static Im1ConnectionVerifyResult Map(TppClient.TppApiObjectResponse<AuthenticateReply> response, ILogger<TppIm1ConnectionService> logger)
        {
            var keyMapping = TppErrorMapper.Map(logger, response, KeyToEnumMapper);
            return keyMapping !=null
                ? new Im1ConnectionVerifyResult.ErrorCase(keyMapping.Value) 
                : (Im1ConnectionVerifyResult) new Im1ConnectionVerifyResult.UnmappedErrorWithStatusCode();
        }
    }
}
