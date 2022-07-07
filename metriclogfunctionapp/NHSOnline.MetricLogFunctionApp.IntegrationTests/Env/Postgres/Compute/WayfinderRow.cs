using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute
{
    internal sealed class WayfinderRow : ITableRow
    {
        public DateTime Date { get; set; }
        public int TotalSessions { get; set; }
        public int TotalViews { get; set; }
        public int Users { get; set; }
        public int TotalReferrals { get; set; }
        public int TotalUpcomingAppointments { get; set; }

        public string InsertSql(string tableName) =>
            @$"INSERT INTO {tableName}(" +
            @$"""Date"", ""TotalSessions"", ""TotalViews""," +
            @$"""Users"", ""TotalReferrals"", ""TotalUpcomingAppointments""" +
            ") " +
            "VALUES(" +
            @$"@Date, @TotalSessions, @TotalViews, " +
            @$"@Users, @TotalReferrals, @TotalUpcomingAppointments" + ")";
    }
}
