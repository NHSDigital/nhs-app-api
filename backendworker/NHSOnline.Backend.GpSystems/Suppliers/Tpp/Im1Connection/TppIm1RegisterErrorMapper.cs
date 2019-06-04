using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection
{
    public class TppIm1RegisterErrorMapper : Im1ConnectionErrorMapper
    {
        private static Dictionary<string, Code> KeyToEnumMapper =>
            new Dictionary<string, Code>
            {
                { "2008", Code.InvalidLinkageDetailsTpp },
                { "2006", Code.InvalidLinkageDetailsTpp },
            };

        protected override Code UnknownError => Code.UnknownError;

        public Im1ConnectionRegisterResult Map(TppClient.TppApiObjectResponse<LinkAccountReply> response, ILogger<TppIm1ConnectionService> logger)
        {
            logger.LogTppErrorResponse(response);
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var keyMapping = TppErrorMapper.Map(logger, response, KeyToEnumMapper);
            
            return keyMapping !=null
                ? new Im1ConnectionRegisterResult.ErrorCase(keyMapping.Value) 
                : MapUnknownError(response.StatusCode);
        }
    }
}
