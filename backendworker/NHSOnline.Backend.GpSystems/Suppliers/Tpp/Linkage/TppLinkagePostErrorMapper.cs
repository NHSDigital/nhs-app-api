using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    internal static class TppLinkagePostErrorMapper
    {
        private static Dictionary<string, Im1ConnectionErrorCodes.InternalCode> KeyToEnumMapper=>
            new Dictionary<string, Im1ConnectionErrorCodes.InternalCode>
            {
                {
                    "2008",
                    Im1ConnectionErrorCodes.InternalCode.ProblemLinkingAccount
                },
                {
                    "2006",
                    Im1ConnectionErrorCodes.InternalCode.ProblemLinkingAccount
                },
                {
                    "2005",
                    Im1ConnectionErrorCodes.InternalCode.InvalidProviderId
                }
            };

        public static LinkageResult Map(TppApiObjectResponse<AddNhsUserResponse> response, ILogger<TppLinkageService> logger)
        {
            var keyMapping = TppErrorMapper.Map(logger, response, KeyToEnumMapper);
            return keyMapping != null
                ? new LinkageResult.ErrorCase(keyMapping.Value)
                : (LinkageResult) new LinkageResult.UnmappedErrorWithStatusCode(response.StatusCode);
        }
    }
}
