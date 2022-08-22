using System;
using System.Threading.Tasks;

namespace NHSOnline.MetricLogFunctionApp.Compute.DeviceInfo;

public interface IDeviceInfoCompute
{
    public Task Execute(DateTime startDateTime, DateTime endDateTime);
}
