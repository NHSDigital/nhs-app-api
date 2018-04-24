using System.Collections.Generic;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;

namespace NHSOnline.Backend.Worker.Stubs.Journeys.Emis.Session
{
    internal class PaulSmithSessionCreateJourney : Journey
    {
        internal override IEnumerable<Mapping> GetMappings()
        {
            const string endUserSessionId = "7YjG1LYkOkSY1iAcXGG8ZU";
            const string sessionId = "AJYF0ufQI6tTpdfwaXAt";
            const string accessIdentityGuid = "28681a98-e280-4038-af63-d5ad39f2833c";
            const string odsCode = "A29928";
            const string title = "Mr";
            const string firstName = "Paul";
            const string surname = "Smith";

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
