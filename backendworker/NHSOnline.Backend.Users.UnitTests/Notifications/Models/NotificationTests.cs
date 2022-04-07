using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Users.Notifications.Models;

namespace NHSOnline.Backend.Users.UnitTests.Notifications.Models
{
    [TestClass]
    public class NotificationTests
    {
        [TestMethod]
        public void ToDictionary_WithAllValues_ReturnsCompleteDictionary()
        {
            // Arrange
            var notification = new NotificationRequest
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = "body",
                Url = new Uri("https://www.example.com")
            };

            var expectedDictionary = new Dictionary<string, string>
            {
                { "title", "title" },
                { "subtitle", "subtitle" },
                { "body", "body" },
                { "url", "https://www.example.com/" },
            };

            // Act
            var result = notification.ToDictionary();

            // Assert
            result.Should().BeEquivalentTo(expectedDictionary);
        }

        [TestMethod]
        public void ToDictionary_WithNoUrl_ReturnsNullUrlDictionary()
        {
            // Arrange
            var notification = new NotificationRequest
            {
                Title = "title",
                Subtitle = "subtitle",
                Body = "body",
                Url = null
            };

            var expectedDictionary = new Dictionary<string, string>
            {
                { "title", "title" },
                { "subtitle", "subtitle" },
                { "body", "body" },
                { "url", null },
            };

            // Act
            var result = notification.ToDictionary();

            // Assert
            result.Should().BeEquivalentTo(expectedDictionary);
        }
    }
}