using System;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace NHSOnline.Backend.Repository
{
    public class UpdateRecordBuilder<TRecord>
    {
        private UpdateDefinition<TRecord> _update = null;

        public UpdateRecordBuilder<TRecord> Set<TField>(
            Expression<Func<TRecord, TField>> field,
            TField value)
        {
            if (_update == null)
            {
                var updateBuilder = Builders<TRecord>.Update;
                _update = updateBuilder.Set(field, value);
            }
            else
            {
                _update.Set(field, value);
            }

            return this;
        }

        internal UpdateDefinition<TRecord> Build()
        {
            return _update;
        }
    }
}