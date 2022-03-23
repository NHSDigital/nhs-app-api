using System;
using System.Data;
using Dapper;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres
{
    internal sealed class DateTimeOffsetTypeHandler : SqlMapper.TypeHandler<DateTimeOffset>
    {
        public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
        {
            parameter.Value = value;
        }

        public override DateTimeOffset Parse(object value)
        {
            return value switch
            {
                DateTime dateTime => new DateTimeOffset(dateTime.ToUniversalTime(), TimeSpan.Zero),
                DateTimeOffset dateTimeOffset => dateTimeOffset,
                _ => throw new InvalidOperationException("Must be DateTime or DateTimeOffset object to be mapped.")
            };
        }
    }
}