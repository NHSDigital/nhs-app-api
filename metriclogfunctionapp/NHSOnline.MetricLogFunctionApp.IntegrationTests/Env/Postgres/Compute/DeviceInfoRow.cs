using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute;

public class DeviceInfoRow : ITableRow
{
    public DateTime Date { get; set; }
    public string AppVersion { get; set; }
    public string DeviceManufacturer { get; set; }
    public string DeviceModel { get; set; }
    public string DeviceOS { get; set; }
    public string DeviceOSVersion { get; set; }
    public string Access { get; set; }
    public int Sessions { get; set; }

    public string InsertSql(string tableName)
        => @$"INSERT INTO {tableName} (" +
           @"""Date"", ""AppVersion"", ""DeviceManufacturer"", ""DeviceModel"", ""DeviceOS"", ""DeviceOSVersion"", ""Access"", ""Sessions""" +
           ")" +
           "VALUES(" +
           @$"@Date, @AppVersion, @DeviceManufacturer, @DeviceModel, @DeviceOS, @DeviceOSVersion, @Access, @Sessions";
}
