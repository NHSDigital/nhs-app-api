using System;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Cache.Messages;

namespace NHSOnline.Backend.Messages.UnitTests.Cache
{
    [TestClass]
    public class SenderCacheProviderTests
    {
        private const string SenderDataCacheKey = "_sender:";
        private const string SenderId = "SENDER_ID";
        private const string SenderName = "Sender Name";

        private ISenderCacheProvider _systemUnderTest;
        private Mock<IMemoryCache> _mockMemoryCache;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMemoryCache = new Mock<IMemoryCache>();

            _systemUnderTest = new SenderCacheProvider(_mockMemoryCache.Object);
        }

        [TestMethod]
        public void GetSender_WhenSenderIdNotFoundInCache_ReturnsNull()
        {
            // Arrange
            object sender = new Sender
            {
                Id = SenderId,
                Name = SenderName
            };

            const string cacheKey = SenderDataCacheKey + SenderId;

            _mockMemoryCache
                .Setup(c => c.TryGetValue(cacheKey, out sender))
                .Returns(false);

            // Act
            var result = _systemUnderTest.GetSender(SenderId);

            // Assert
            result.Should().Be(null);
            _mockMemoryCache.VerifyAll();
        }

        [TestMethod]
        public void GetSender_WhenSenderIdFoundInCache_ReturnsSender()
        {
            // Arrange
            object sender = new Sender
            {
                Id = SenderId,
                Name = SenderName
            };

            const string cacheKey = SenderDataCacheKey + SenderId;

            _mockMemoryCache
                .Setup(c => c.TryGetValue(cacheKey, out sender))
                .Returns(true);

            // Act
            var result = _systemUnderTest.GetSender(SenderId);

            // Assert
            result.Should().BeEquivalentTo(sender);
            _mockMemoryCache.VerifyAll();
        }

        [TestMethod]
        public void SetSender_IsCachedWithExpectedCacheOptions()
        {
            // Arrange
            var sender = new Sender
            {
                Id = SenderId,
                Name = SenderName
            };

            var mockCacheEntry = new Mock<ICacheEntry>();
            string cacheEntryKey = null;
            Sender cacheEntryValue = null;
            DateTimeOffset? cacheExpirationDateTime = null;
            long? cacheSize = long.MinValue;

            const string cacheKey = SenderDataCacheKey + SenderId;

            _mockMemoryCache
                .Setup(c => c.CreateEntry(cacheKey))
                .Callback((object entry) => cacheEntryKey = (string) entry)
                .Returns(mockCacheEntry.Object);

            mockCacheEntry
                .SetupSet(entry => entry.Value = It.IsAny<object>())
                .Callback<object>(entryValue => cacheEntryValue = (Sender) entryValue);

            mockCacheEntry
                .SetupSet(entry => entry.AbsoluteExpiration = It.IsAny<DateTimeOffset?>())
                .Callback<DateTimeOffset?>(relativeDate => cacheExpirationDateTime = relativeDate);

            mockCacheEntry
                .SetupSet(entry => entry.Size = It.IsAny<long?>())
                .Callback<long?>(size => cacheSize = size);

            // Act
            _systemUnderTest.SetSender(sender);

            // Assert
            cacheEntryKey.Should().Be(cacheKey);
            cacheEntryValue.Should().BeEquivalentTo(sender);
            cacheExpirationDateTime.Should().BeCloseTo(DateTimeOffset.Now,TimeSpan.FromHours(1));
            cacheSize.Should().Be(sender.CacheSize);

            _mockMemoryCache.VerifyAll();
            mockCacheEntry.VerifyAll();
        }
    }
}