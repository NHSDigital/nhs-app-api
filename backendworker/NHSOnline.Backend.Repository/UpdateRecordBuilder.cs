using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace NHSOnline.Backend.Repository
{
    public class UpdateRecordBuilder<TRecord>
    {
        private readonly IList<UpdateDefinition<TRecord>> _updateValues = new List<UpdateDefinition<TRecord>>();

        public UpdateRecordBuilder<TRecord> Set<TField>(
            Expression<Func<TRecord, TField>> field,
            TField value)
        {
            var updateBuilder = Builders<TRecord>.Update;
            var update = updateBuilder.Set(field, value);
            _updateValues.Add(update);

            return this;
        }

        public UpdateDefinition<TRecord> Build()
        {
            return Builders<TRecord>.Update.Combine(_updateValues);
        }
    }
}