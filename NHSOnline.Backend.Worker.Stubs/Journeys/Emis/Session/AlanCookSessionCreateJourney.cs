using System.Collections.Generic;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;

namespace NHSOnline.Backend.Worker.Stubs.Journeys.Emis.Session
{
    internal class AlanCookSessionCreateJourney : Journey
    {
        internal override IEnumerable<Mapping> GetMappings()
        {
            const string endUserSessionId = "7YjG1LYkOkSY1iAcXGG8ZU";
            const string sessionId = "fbWgorZ8Fggk9c5PgKd7";
            const string accessIdentityGuid = "7e14cfb4-eb7a-44c3-8603-28ee36c7a9bf";
            const string odsCode = "A29928";
            const string title = "Mr";
            const string firstName = "Alan";
            const string surname = "Cook";

            yield return EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId);

            yield return SessionConfigurator
                .ForRequest(accessIdentityGuid, odsCode, endUserSessionId)
                .RespondWithSuccess(sessionId, title, firstName, surname, accessIdentityGuid, odsCode,
                    AssociationType.Self);
        }
    }
}