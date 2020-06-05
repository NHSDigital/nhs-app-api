using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace NHSOnline.Backend.PfsApi.UnitTests.TermsAndConditions
{
    internal class UpdateDefinitionAssertor<TRecord>
    {
        private readonly IList<Tuple<string, string>> _expectedUpdates = new List<Tuple<string, string>>();

        public UpdateDefinitionAssertor<TRecord> AddExpectedUpdate(string field, string value)
        {
            _expectedUpdates.Add(Tuple.Create(field, value));
            return this;
        }

        public void Assert(UpdateDefinition<TRecord> actualUpdate)
        {
            var expectedBody = string.Join(", ", _expectedUpdates.Select(x => $"\"{x.Item1}\" : {x.Item2}"));
            var renderedUpdate = actualUpdate.Render(BsonSerializer.LookupSerializer<TRecord>(),
                new BsonSerializerRegistry());
            renderedUpdate.ToString().Should().BeEquivalentTo($"{{ \"$set\" : {{ {expectedBody} }} }}");
        }
    }
}