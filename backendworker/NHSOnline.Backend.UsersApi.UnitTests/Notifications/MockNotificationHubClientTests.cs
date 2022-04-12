using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Notifications.Extensions;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class MockNotificationHubClientTests
    {
        private MockNotificationHubClient _hubClient;

        [TestInitialize]
        public void TestInitialise()
        {
            _hubClient = new MockNotificationHubClient(0);
        }

        [TestMethod]
        public async Task GetRegistrationsByChannelAsync_Returns_EmptyCollection()
        {
            var result = await _hubClient.GetRegistrationsByChannelAsync(string.Empty, Int32.MaxValue);
            
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetRegistrationsByTagAsync_Returns_Collection_With_ProvidedTag()
        {
            var tag = NhsLoginTagGenerator.Generate("test_nhs_loginId");
            var result =
                await _hubClient.GetRegistrationsByTagAsync(tag,
                    Int32.MaxValue);

            result.Count().Should().Be(1);
            var tags = result.SelectMany(rd => rd.Tags).ToList();
            
            tags.Should().Contain(tag);
            result.InstallationIds().Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task SendTemplateNotificationAsync_Returns_Predefined_Notification_Outcome()
        {
            var result=await _hubClient.SendTemplateNotificationAsync(new Dictionary<string, string>());
            
            result.NotificationId.Should().NotBeNullOrEmpty();
        }
        
        [TestMethod]
        public async Task GetNotificationOutcomeDetailsAsync_Returns_Predefined_Detailed_Notification_Outcome()
        {
            var result=await _hubClient.GetNotificationOutcomeDetailsAsync("notificationId");

            result.ApnsOutcomeCounts.Should().NotBeNull();
            result.FcmOutcomeCounts.Should().NotBeNull();
        }
    }
}