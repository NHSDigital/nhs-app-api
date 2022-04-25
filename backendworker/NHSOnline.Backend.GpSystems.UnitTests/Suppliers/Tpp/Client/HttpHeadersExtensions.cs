using FluentAssertions;
using FluentAssertions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client
{
    internal static class HttpHeadersExtensions
    {
        public static AndWhichConstraint<
                GenericDictionaryAssertions<IEnumerable<KeyValuePair<string, IEnumerable<string>>>, string,
                    IEnumerable<string>>,
                KeyValuePair<string, IEnumerable<string>>>
            ContainHeader(
                this GenericDictionaryAssertions<IEnumerable<KeyValuePair<string, IEnumerable<string>>>, string,
                    IEnumerable<string>> headers,
                string name, string value)
        {
            return headers.ContainSingle(kvp =>
                    kvp.Key.Equals(name, StringComparison.Ordinal)
                    && kvp.Value.SingleOrDefault().Equals(value, StringComparison.Ordinal),
                "headers should contain header " + name + " with value " + value);
        }
    }
}