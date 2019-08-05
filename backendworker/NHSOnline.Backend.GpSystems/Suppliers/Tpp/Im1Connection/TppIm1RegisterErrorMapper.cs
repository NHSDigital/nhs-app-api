using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection
{
    public static class TppIm1RegisterErrorMapper
    {
        private static Dictionary<string, InternalCode> KeyToEnumMapper =>
            new Dictionary<string, InternalCode>
            {
                { "2008", InternalCode.InvalidLinkageDetailsTpp },
                { "2006", InternalCode.InvalidLinkageDetailsTpp },
            };

        public static Im1ConnectionRegisterResult Map(TppClient.TppApiObjectResponse<LinkAccountReply> response, ILogger<TppIm1ConnectionService> logger)
        {
            var keyMapping = TppErrorMapper.Map(logger, response, KeyToEnumMapper);
            return keyMapping !=null
                ? new Im1ConnectionRegisterResult.ErrorCase(keyMapping.Value) 
                : (Im1ConnectionRegisterResult) new Im1ConnectionRegisterResult.UnmappedErrorWithStatusCode();
        }
    }
}
