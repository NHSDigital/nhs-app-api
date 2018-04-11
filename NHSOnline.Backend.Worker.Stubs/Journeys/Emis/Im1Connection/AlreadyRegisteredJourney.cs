using System.Collections.Generic;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;

namespace NHSOnline.Backend.Worker.Stubs.Journeys.Emis.Im1Connection
{
    internal class AlreadyRegisteredJourney : Journey
    {
        internal override IEnumerable<Mapping> GetMappings()
        {
            const string accessIdentityGuid = "1da4fe9d-0fd2-45bc-90a9-014e57291d0f";
            const string odsCode = "A29928";
            const string title = "Miss";
            const string firstName = "Halle";
            const string surname = "Dawe";
            const string dateOfBirth = "1994-02-21T00:00:00";
            const string nhsNumber = "2227007273";
            const string accountId = "4937786121";
            const string linkageKey = "tTALtBP3rLR16";
            const string endUserSessionId = "7YjG1LYkOkSY1iAcXGG8ZU";
            const string sessionId = "4RDwmQVi3OfSbp47dbAnRF";
            const string userPatientLinkToken = "DbLYlUrwyGpgZ65Mlk6601";

            yield return EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId);

            yield return MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithUserAlreadyLinked();

            yield return SessionConfigurator
                .ForRequest(accessIdentityGuid, odsCode, endUserSessionId)
                .RespondWithSuccess(sessionId, title, firstName, surname, userPatientLinkToken, odsCode, AssociationType.Self);

            yield return DemographicsConfigurator
                .ForRequest(endUserSessionId, sessionId, userPatientLinkToken)
                .RespondWithSuccess(title, firstName, surname, new[] { PatientIdentifier.NHSNumber(nhsNumber) });
        }
    }
}
