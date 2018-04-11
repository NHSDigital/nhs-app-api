using System.Collections.Generic;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;

namespace NHSOnline.Backend.Worker.Stubs.Journeys.Emis.Im1Connection
{
    internal class SuccessfulIm1RegistrationJourney : Journey
    {
        internal override IEnumerable<Mapping> GetMappings()
        {
            const string accessIdentityGuid = "7a3a3cf8-a797-4fcc-a4b9-629cdbe104fc";
            const string odsCode = "A29928";
            const string title = "Mr";
            const string firstName = "Montel";
            const string surname = "Frye";
            const string dateOfBirth = "1972-04-12T00:00:00";
            const string nhsNumber = "0968764215";
            const string accountId = "4140044939";
            const string linkageKey = "vVGO8bgV6fvPb";
            const string endUserSessionId = "7YjG1LYkOkSY1iAcXGG8ZU";
            const string sessionId = "2jM47sZ0ic4FIAcVogI4WI";
            const string userPatientLinkToken = "gpSWtREiH9499bPzix8v5b";

            yield return EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId);

            yield return MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithSuccess(accessIdentityGuid);

            yield return SessionConfigurator
                .ForRequest(accessIdentityGuid, odsCode, endUserSessionId)
                .RespondWithSuccess(sessionId, title, firstName, surname, userPatientLinkToken, odsCode, AssociationType.Self);

            yield return DemographicsConfigurator
                .ForRequest(endUserSessionId, sessionId, userPatientLinkToken)
                .RespondWithSuccess(title, firstName, surname, new[] { PatientIdentifier.NHSNumber(nhsNumber) });
        }
    }
}
