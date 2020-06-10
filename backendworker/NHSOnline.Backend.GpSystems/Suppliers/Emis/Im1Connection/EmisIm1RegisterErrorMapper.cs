using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Im1Connection
{
    public static class EmisIm1RegisterErrorMapper
    {
        private static KeyAndMessageToEnumMapper<InternalCode>
            KeyAndMessageToError =>
            new KeyAndMessageToEnumMapper<InternalCode>()
                .Add("4031030", 
                    "Patient Facing Services API v2 is not enabled at this practice",
                    InternalCode.PatientFacingServicesApiv2IsNotEnabledAtThisPractice)
                .Add("4031030", 
                    "Patient Facing Services are not enabled by this practice",
                    InternalCode.PatientFacingServicesAreNotEnabledAtThisPractice)
                .Add("400", 
                    "AccountId length outside of valid range",
                    InternalCode.AccountIdLengthOutsideOfValidRange)
                .Add("400", 
                    "LinkageKey length outside of valid range",
                    InternalCode.LinkageKeyLengthOutsideOfValidRange)
                .Add("400", 
                    "length outside of valid range",
                    InternalCode.InputValueLengthOutsideOfValidRange)
                .AddMessageToEnum("Registered online user is already linked",InternalCode.UserAlreadyLinked)
                .AddMessageToEnum("No registered online user found for given linkage details",InternalCode.NoUserFoundForLinkageDetails)
                .AddMessageToEnum("Invalid linkage details", InternalCode.InvalidLinkageDetails)
                .AddMessageToEnum("The request is invalid", InternalCode.InvalidLinkageDetails)
                .AddMessageToEnum("No match found for given demographics",InternalCode.NoMatchFoundForGivenDemographics)
                .AddKeyToEnum("4041104", InternalCode.NoUserFoundForLinkageDetails)
                .AddKeyToEnum("4001105", InternalCode.InvalidLinkageDetails)
                .AddKeyToEnum("4001106", InternalCode.NoMatchFoundForGivenDemographics)
                .AddKeyToEnum("4001001", InternalCode.UnableToFindOrganisation)
                .AddKeyToEnum("4001107", InternalCode.UserAccountIsInactiveOrArchived)
                .AddKeyToEnum("4001401", InternalCode.PracticeNotLive)
                .AddKeyToEnum("4001552", InternalCode.PatientArchived)
                .AddKeyToEnum("4001108", InternalCode.UserAlreadyLinked);

        public static Im1ConnectionRegisterResult Map(EmisApiObjectResponse<MeApplicationsPostResponse> response,
            ILogger<EmisIm1ConnectionService> logger)
        {
            var mappedValue = EmisErrorMapper.Map(logger, response, KeyAndMessageToError);

            return mappedValue != null
                ? new Im1ConnectionRegisterResult.ErrorCase(mappedValue.Value)
                : (Im1ConnectionRegisterResult) new Im1ConnectionRegisterResult.UnmappedErrorWithStatusCode();
        }
    }
}
