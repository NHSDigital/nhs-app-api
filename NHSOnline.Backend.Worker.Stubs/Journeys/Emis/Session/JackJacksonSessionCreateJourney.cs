using System.Collections.Generic;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;

namespace NHSOnline.Backend.Worker.Stubs.Journeys.Emis.Session
{
    internal class JackJacksonSessionCreateJourney : Journey
    {
        internal override IEnumerable<Mapping> GetMappings()
        {
            const string endUserSessionId = "7YjG1LYkOkSY1iAcXGG8ZU";
            const string sessionId = "gY39SJJMEEg7rNbcsfF8";
            const string accessIdentityGuid = "efa22020-9221-46a6-a0f0-6c0340b8f44d";
            const string odsCode = "A29928";
            const string title = "Mr";
            const string firstName = "Jack";
            const string surname = "Jackson";

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
