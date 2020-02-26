using FluentAssertions;
using FluentAssertions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client
{
    internal static class HttpHeadersExtensions
    {
        public static AndWhichConstraint<GenericCollectionAssertions<KeyValuePair<string, IEnumerable<string>>>, KeyValuePair<string, IEnumerable<string>>>
            ContainHeader(this GenericCollectionAssertions<KeyValuePair<string, IEnumerable<string>>> headers, string name, string value)
        {
            return headers.Contain(
                kvp => IsHeader(kvp, name, value),
                "headers should contain header " + name + " with value " + value);
        }

        private static bool IsHeader(KeyValuePair<string, IEnumerable<string>> kvp, string name, string value)
            => string.Equals(kvp.Key, name, StringComparison.Ordinal) &&
                kvp.Value.Count() == 1 &&
                string.Equals(kvp.Value.Single(), value, StringComparison.Ordinal);
    }
}