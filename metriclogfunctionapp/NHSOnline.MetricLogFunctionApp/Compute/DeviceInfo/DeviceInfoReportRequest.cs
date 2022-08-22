using System;

namespace NHSOnline.MetricLogFunctionApp.Compute.DeviceInfo;

public class DeviceInfoReportRequest
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
}
