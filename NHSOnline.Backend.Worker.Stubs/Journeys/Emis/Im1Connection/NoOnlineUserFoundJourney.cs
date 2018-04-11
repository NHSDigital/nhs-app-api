using System.Collections.Generic;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Mocking.Models;

namespace NHSOnline.Backend.Worker.Stubs.Journeys.Emis.Im1Connection
{
    internal class NoOnlineUserFoundJourney : Journey
    {
        internal override IEnumerable<Mapping> GetMappings()
        {
            const string odsCode = "A29928";
            const string surname = "Diaz";
            const string dateOfBirth = "1991-10-30T00:00:00";
            const string accountId = "3834473016";
            const string linkageKey = "QAkP7SUrfsQ1l";
            const string endUserSessionId = "7YjG1LYkOkSY1iAcXGG8ZU";

            yield return EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId);

            yield return MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithNoOnlineUserFound();
        }
    }
}
