using System.Linq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests
{
    public class HomeScreenBuilder
    {
        private readonly HomeScreen _homeScreen = new HomeScreen();

        public HomeScreenBuilder PublicHealthNotifications(params PublicHealthNotification[] publicHealthNotifications)
        {
            _homeScreen.PublicHealthNotifications = publicHealthNotifications.ToList();
            return this;
        }

        public HomeScreen Build()
        {
            return _homeScreen;
        }
    }
}