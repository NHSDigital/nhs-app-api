using System;
using System.Collections.Generic;
using System.Globalization;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public static class DictionaryExtensions
    {
        public static void AddIfValueNotNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if ((object)value != null)
                dictionary.Add(key, value);
        }
    }
}