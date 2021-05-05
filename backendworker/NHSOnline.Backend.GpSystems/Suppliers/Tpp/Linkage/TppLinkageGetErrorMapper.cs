using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    internal static class TppLinkageGetErrorMapper
    {
        private static Dictionary<string, Im1ConnectionErrorCodes.InternalCode> KeyToEnumMapper =>
            new Dictionary<string, Im1ConnectionErrorCodes.InternalCode>
            {
                {
                    "2005",
                    Im1ConnectionErrorCodes.InternalCode.InvalidProviderId
                },
                {
                    "2006",
                    Im1ConnectionErrorCodes.InternalCode.ProblemLinkingAccount
                },
                {
                    "2008",
                    Im1ConnectionErrorCodes.InternalCode.ProblemLinkingAccount
                },
                {
                    "20019",
                    Im1ConnectionErrorCodes.InternalCode.PatientOnSystemOneNotMatchedToARecordOnPDS
                },
                {
                    "200509",
                    Im1ConnectionErrorCodes.InternalCode.IncompleteOrEndedPFSRegistrationDetails
                },
                {
                    "200512",
                    Im1ConnectionErrorCodes.InternalCode.ProvidedLastNameDoesNotMatchSystmOne
                },
                {
                    "200513",
                    Im1ConnectionErrorCodes.InternalCode.ProvidedDOBDoesNotMatchSystmOne
                },
                {
                    "200553",
                    Im1ConnectionErrorCodes.InternalCode.PatientIsNotOldEnoughToSignUp
                },
                {
                    "200554",
                    Im1ConnectionErrorCodes.InternalCode.NoPatientWithNhsNumberExistsOnSystmOne
                },
                {
                    "200555",
                    Im1ConnectionErrorCodes.InternalCode.PatientNotRegisteredAtPracticeSpecifiedByOrgCode
                }
            };

        public static LinkageResult Map(TppApiObjectResponse<LinkAccountReply> response, ILogger<TppLinkageService> logger)
        {
            var keyMapping = TppErrorMapper.Map(logger, response, KeyToEnumMapper);
            return keyMapping != null
                ? new LinkageResult.ErrorCase(keyMapping.Value)
                : (LinkageResult) new LinkageResult.UnmappedErrorWithStatusCode(response.StatusCode);
        }
    }
}
