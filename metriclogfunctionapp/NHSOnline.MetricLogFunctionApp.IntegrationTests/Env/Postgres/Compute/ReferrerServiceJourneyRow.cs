using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute;

internal sealed class ReferrerServiceJourneyRow : ITableRow
{
    public DateTime Date { get; set; }
    public string ReferrerId { get; set; }
    public int RecordViews { get; set; }
    public int Prescriptions { get; set; }
    public int OdRegistrations { get; set; }
    public int OdWithdrawals { get; set; }
    public int OdUpdates { get; set; }
    public int OdLookups { get; set; }
    public int NomPharmacyUpdate { get; set; }
    public int NomPharmacyCreate { get; set; }
    public int AppointmentsBooked { get; set; }
    public int AppointmentsCancelled { get; set; }
    public int RecordViewsDcr { get; set; }
    public int RecordViewsScr { get; set; }
    public int SilverIntegrationJumpOffs { get; set; }
    public int CovidPassJumpOffs { get; set; }

    public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Date"", ""ReferrerId"", ""RecordViews"", ""Prescriptions"", ""OdRegistrations"", ""OdWithdrawals"", ""OdUpdates"", ""OdLookups"", ""NomPharmacyUpdate"", ""NomPharmacyCreate"", ""AppointmentsBooked"", ""AppointmentsCancelled"", ""RecordViewsDcr"", ""RecordViewsScr"", ""SilverIntegrationJumpOffs"", ""CovidPassJumpOffs"")
VALUES(@Date, @ReferrerId, @RecordViews, @Prescriptions, @OdRegistrations, @OdWithdrawals, @OdUpdates, @OdLookups, @NomPharmacyUpdate, @NomPharmacyCreate, @AppointmentsBooked, @AppointmentsCancelled, @RecordViewsDcr, @RecordViewsScr, @SilverIntegrationJumpOffs, @CovidPassJumpOffs)
";
}

