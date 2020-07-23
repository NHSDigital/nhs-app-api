using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.PfsApi.Areas.Session.Models
{
    public class PostUserSessionResponse : UserSessionResponse
    {
        public ServiceJourneyRulesResponse ServiceJourneyRules { get; set; }
    }
}