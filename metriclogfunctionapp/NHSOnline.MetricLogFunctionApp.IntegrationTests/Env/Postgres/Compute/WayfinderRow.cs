using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute
{
    internal sealed class WayfinderRow : ITableRow
    {
        public DateTime Date { get; set; }
        public int Sessions { get; set; }
        public int Views { get; set; }
        public int TotalReferrals { get; set; }
        public int TotalAppts { get; set; }
        public int SessionsWithReferrals { get; set; }
        public int SessionsWithAppts { get; set; }
        public int ViewsWithReferrals { get; set; }
        public int ViewsWithAppts { get; set; }
        public int Neither { get; set; }
        public int Both { get; set; }
        public int Either { get; set; }
        public int MostAppts { get; set; }
        public int MostReferrals { get; set; }

        public string InsertSql(string tableName) =>
            @$"INSERT INTO {tableName}(" +
            @$"""Date"", ""Sessions"", ""Views""," +
            @$"""Users"", ""TotalReferrals"", ""TotalAppts""" +
            @$"""SessionsWithReferrals"", ""SessionsWithAppts"", ""ViewsWithReferrals""" +
            @$"""ViewsWithAppts"", ""Neither"", ""Both""" +
            @$"""Either"", ""MostAppts"", ""MostReferrals""" +
            ") " +
            "VALUES(" +
            @$"@Date, @Sessions, @Views, " +
            @$"@Users, @TotalReferrals, @TotalAppts" +
            @$"@SessionsWithReferrals, @SessionsWithAppts, @ViewsWithReferrals" +
            @$"@ViewsWithAppts, @Neither, @Both" +
            @$"@Either, @MostAppts, @MostReferrals" + ")";
    }
}
