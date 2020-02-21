using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Support
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> InRandomOrder<T>(this IEnumerable<T> list)
        {
            var shuffledList = new List<T>(list);
            return shuffledList.OrderBy(x => Guid.NewGuid());
        }

        public static Dictionary<TKey, TElement> ToDictionaryLogOnFailure<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, ILogger logger)
        {
            try
            {
                return source.ToDictionary(keySelector, elementSelector);
            }
            catch (ArgumentException)
            {
                LogDuplicates(source, keySelector, elementSelector, logger);
                throw;
            }
        }

        private static void LogDuplicates<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
            ILogger logger)
        {
            try
            {
                if (source == null)
                {
                    return;
                }

                var duplicates = source
                    .Select(x => new { Key = keySelector(x), Element = elementSelector(x) })
                    .GroupBy(x => x.Key)
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g)
                    .ToArray();

                if (duplicates.Any())
                {
                    var duplicateJson = JsonConvert.SerializeObject(duplicates);
                    logger.LogInformation("Duplicate keys found when building dictionary: " + duplicateJson);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unhandled exception occurred when logging duplicate dictionary entries");
            }
        }
    }
}