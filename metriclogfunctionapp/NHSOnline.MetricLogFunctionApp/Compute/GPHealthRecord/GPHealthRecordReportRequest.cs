using System;

namespace NHSOnline.MetricLogFunctionApp.Compute.GPHealthRecord;

public class GPHealthRecordReportRequest
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
}
