using System.Collections.Generic;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public static class DictionaryExtensions
    {
        public static void AddIfValueNotNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (value != null)
                dictionary.Add(key, value);
        }
    }
}