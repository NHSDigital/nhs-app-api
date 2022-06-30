using System;
using Microsoft.Extensions.Caching.Memory;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Cache.Messages
{
    public class SenderCacheProvider : ISenderCacheProvider
    {
        private readonly IMemoryCache _cache;

        public SenderCacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SetSender(Sender sender)
        {
            var cacheKey = GetCacheKey(sender);

            _cache.Set(
                cacheKey,
                sender,
                new MemoryCacheEntryOptions
                {
                    Size = sender.CacheSize,
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                });
        }

        public Sender GetSender(string senderId)
        {
            var cacheKey = GetCacheKey(senderId);

            var item = _cache.TryGetValue(cacheKey, out Sender sender);

            return (item) ? sender : null;
        }

        private static string GetCacheKey(Sender sender)
        {
            return GetCacheKey(sender.Id);
        }

        private static string GetCacheKey(string senderId)
        {
            return $"_sender:{senderId}";
        }
    }
}