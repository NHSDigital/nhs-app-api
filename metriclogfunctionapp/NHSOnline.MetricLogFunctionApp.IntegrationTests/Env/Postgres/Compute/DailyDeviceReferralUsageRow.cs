using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute
{
    public class DailyDeviceReferralUsageRow :ITableRow
    {
        public DateTime Date { get; set; }
        public string DeviceOS { get; set; }
        public string Referral { get; set; }
        public int Users { get; set; }
        public int Logins { get; set; }
        public int RecordViewsDCR { get; set; }
        public int RecordViewsSCR { get; set; }
        public int Prescriptions { get; set; }
        public int NomPharmacy { get; set; }
        public int AppointmentsBooked { get; set; }
        public int AppointmentsCancelled { get; set; }
        public int ODRegistrations { get; set; }
        public int ODWithdrawals { get; set; }
        public string ReferralOrigin { get; set; }

        public string InsertSql(string tableName)
            => @$"INSERT INTO {tableName} (" +
               @"""Date"", ""DeviceOS"", ""Referral"",  ""Users"", ""Logins"", ""RecordViewsDCR"", ""RecordViewsSCR"", ""Prescriptions""," +
               @"""NomPharmacy"",""AppointmentsBooked"", ""AppointmentsCancelled,"", ""ODRegistrations,"",""ODWithdrawals,"" ""ReferralOrigin""" +
               ")" +
               "VALUES(" +
               @$"@Date, @DeviceOS, @Referral, @Users, @Logins, @RecordViewsDCR, @RecordViewsSCR, @Prescriptions," +
               @$"@NomPharmacy, @AppointmentsBooked, @AppointmentsCancelled, @ODRegistrations, @ODWithdrawals, @ReferralOrigin";
    }
}