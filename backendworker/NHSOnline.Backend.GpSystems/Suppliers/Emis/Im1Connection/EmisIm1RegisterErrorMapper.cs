using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Im1Connection
{
    public class EmisIm1RegisterErrorMapper : Im1ConnectionErrorMapper
    {
        protected override Code UnknownError =>
            Code.UnknownError;

        protected static KeyAndMessageToEnumMapper<Code>
            KeyAndMessageToError =>
            new KeyAndMessageToEnumMapper<Code>()
                .Add("4031030", 
                    "Patient Facing Services API v2 is not enabled at this practice",
                    Code.PatientFacingServicesApiv2IsNotEnabledAtThisPractice)
                
                .Add("4031030", 
                    "Patient Facing Services are not enabled by this practice",
                    Code.PatientFacingServicesAreNotEnabledAtThisPractice)
                
                .Add("400", 
                    "AccountId length outside of valid range",
                    Code.AccountIdLengthOutsideOfValidRange)
                
                .Add("400", 
                    "LinkageKey length outside of valid range",
                    Code.LinkageKeyLengthOutsideOfValidRange)
                
                .Add("400", 
                    "length outside of valid range",
                    Code.InputValueLengthOutsideOfValidRange)
                
                .AddKeyToEnum("Registered online user is already linked",Code.UserAlreadyLinked)
                .AddKeyToEnum("No registered online user found for given linkage details",Code.NoUserFoundForLinkageDetails)
                .AddKeyToEnum("Invalid linkage details", Code.InvalidLinkageDetails)
                .AddKeyToEnum("No match found for given demographics",Code.NoMatchFoundForGivenDemographics)
                .AddKeyToEnum("4041104", Code.NoUserFoundForLinkageDetails)
                .AddKeyToEnum("4001105", Code.InvalidLinkageDetails)
                .AddKeyToEnum("4001106", Code.NoMatchFoundForGivenDemographics)
                .AddKeyToEnum("4001001", Code.UnableToFindOrganisation)
                .AddKeyToEnum("4001107", Code.UserAccountIsInactiveOrArchived)
                .AddKeyToEnum("4001401", Code.PracticeNotLive)
                .AddKeyToEnum("4001552", Code.PatientArchived)
                .AddKeyToEnum("4001108", Code.UserAlreadyLinked);

        public Im1ConnectionRegisterResult Map(EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse> response,
            ILogger<EmisIm1ConnectionService> logger)
        {
            var mappedValue = EmisErrorMapper.Map(logger, response, KeyAndMessageToError);

            return mappedValue != null
                ? new Im1ConnectionRegisterResult.ErrorCase(mappedValue.Value)
                : MapUnknownError(response.StatusCode);
        }
    }
}
