using System;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Compute.DeviceInfo;

public class DeviceInfoCompute : IDeviceInfoCompute
{
    private readonly IComputeLogger<DeviceInfoCompute> _logger;
    private readonly IEventsRepository _repository;

    public DeviceInfoCompute(
        IComputeLogger<DeviceInfoCompute> logger,
        IEventsRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task Execute(DateTime startDateTime, DateTime endDateTime)
    {
        _logger.Started(nameof(DeviceInfoCompute), startDateTime, endDateTime);
        await _repository.CallStoredProcedure("CALL compute.DeviceInfoCompute({0},{1})", startDateTime, endDateTime);
    }
}
