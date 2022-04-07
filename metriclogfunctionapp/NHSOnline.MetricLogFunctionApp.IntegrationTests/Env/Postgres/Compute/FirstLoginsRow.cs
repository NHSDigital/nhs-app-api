using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute
{
    internal sealed class FirstLoginsRow : ITableRow
    {
        public string LoginId { get; set; }
        public DateTime? FirstP5LoginDate { get; set; }
        public DateTime? FirstP9LoginDate { get; set; }
        public DateTime? ConsentDate { get; set; }
        public string ConsentProofLevel { get; set; }
        public DateTimeOffset? FirstP5LoginTimestamp { get; set; }
        public DateTimeOffset? FirstP9LoginTimestamp { get; set; }
        public DateTimeOffset? ConsentTimestamp { get; set; }
        public string LatestOdsCode { get; set; }
        public string LatestProofLevel { get; set; }
        public DateTimeOffset? LatestLoginTimestamp { get; set; }
        public string SingleLoginFlag { get; set; }

        public string InsertSql(string tableName) =>
            @$"INSERT INTO {tableName}(" +
            @$"""LoginId"", ""FirstP5LoginDate"", ""FirstP9LoginDate""," +
            @$"""ConsentDate"", ""ConsentProofLevel"", ""FirstP5LoginTimestamp""," +
            @$"""FirstP9LoginTimestamp"",""ConsentTimestamp""" +
            ") " +
            "VALUES(" +
            @$"@LoginId, @FirstP5LoginDate, @FirstP9LoginDate, " +
            @$"@ConsentDate,  @ConsentProofLevel, @FirstP5LoginTimestamp," +
            @$"@FirstP9LoginTimestamp, @ConsentTimestamp" +
            ")";
    }
}
