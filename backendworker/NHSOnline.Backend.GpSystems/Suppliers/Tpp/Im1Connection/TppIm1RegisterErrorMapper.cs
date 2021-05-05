using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection
{
    internal static class TppIm1RegisterErrorMapper
    {
        private static Dictionary<string, InternalCode> KeyToEnumMapper =>
            new Dictionary<string, InternalCode>
            {
                { "2008", InternalCode.InvalidLinkageDetailsTpp },
                { "2006", InternalCode.InvalidLinkageDetailsTpp },
                { "20019", InternalCode.PatientOnSystemOneNotMatchedToARecordOnPDS },
                { "200509", InternalCode.IncompleteOrEndedPFSRegistrationDetails },
                { "200512", InternalCode.ProvidedLastNameDoesNotMatchSystmOne },
                { "200513", InternalCode.ProvidedDOBDoesNotMatchSystmOne },
                { "200553", InternalCode.PatientIsNotOldEnoughToSignUp },
                { "200554", InternalCode.NoPatientWithNhsNumberExistsOnSystmOne },
                { "200555", InternalCode.PatientNotRegisteredAtPracticeSpecifiedByOrgCode },
                { "200556", InternalCode.ErrorCreatingNewPFSAccountAndLinkageKeys },
            };

        public static Im1ConnectionRegisterResult Map(TppApiObjectResponse<LinkAccountReply> response, ILogger<TppIm1ConnectionService> logger)
        {
            var keyMapping = TppErrorMapper.Map(logger, response, KeyToEnumMapper);
            return keyMapping !=null
                ? new Im1ConnectionRegisterResult.ErrorCase(keyMapping.Value) 
                : (Im1ConnectionRegisterResult) new Im1ConnectionRegisterResult.UnmappedErrorWithStatusCode();
        }
    }
}
