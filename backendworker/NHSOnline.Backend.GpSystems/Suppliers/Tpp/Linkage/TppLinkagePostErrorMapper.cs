using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    public class TppLinkagePostErrorMapper : LinkageErrorMapper
    {
        protected override Im1ConnectionErrorCodes.Code UnknownError => Im1ConnectionErrorCodes.Code.UnknownError;

        private static Dictionary<string, Im1ConnectionErrorCodes.Code> KeyToEnumMapper=>
            new Dictionary<string, Im1ConnectionErrorCodes.Code>
            {
                {
                    "2008",
                    Im1ConnectionErrorCodes.Code.ProblemLinkingAccount
                },
                {
                    "2006",
                    Im1ConnectionErrorCodes.Code.ProblemLinkingAccount
                },
                {
                    "2005",
                    Im1ConnectionErrorCodes.Code.InvalidProviderId
                }
            };
        

        public LinkageResult Map(TppClient.TppApiObjectResponse<AddNhsUserResponse> response, ILogger<TppLinkageService> logger)
        {
            logger.LogTppErrorResponse(response);
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var keyMapping = TppErrorMapper.Map(logger, response, KeyToEnumMapper);

            return keyMapping != null
                ? new LinkageResult.ErrorCase(keyMapping.Value)
                : MapUnknownError(response.StatusCode);
        }
    }
}
