using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class HomeScreen : ICloneable<HomeScreen>
    {
        public List<PublicHealthNotification> PublicHealthNotifications { get; set; }

        public HomeScreen Clone() => new HomeScreen
        {
            PublicHealthNotifications = PublicHealthNotifications?.Select(p => p.Clone()).ToList()
        };
    }
}