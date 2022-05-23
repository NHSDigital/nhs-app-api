using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute
{
    internal sealed class ReferrerLoginRow : ITableRow
    {
        public DateTime Date { get; set; }
        public string ReferrerId { get; set; }
        public int ExistingUsers { get; set; }
        public int NewUsers { get; set; }

        public string InsertSql(string tableName) =>
            @$"INSERT INTO {tableName}(" +
            @$"""Date"", ""ReferrerId"", ""ExistingUsers""," +
            @$"""NewUsers""" +
            ") " +
            "VALUES(" +
            @$"@Date, @ReferrerId, @ExistingUsers, " +
            @$"@NewUsers" + ")";
    }
}
