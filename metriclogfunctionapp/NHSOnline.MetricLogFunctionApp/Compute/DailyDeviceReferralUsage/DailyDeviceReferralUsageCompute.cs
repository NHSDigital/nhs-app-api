using System;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Compute.DailyDeviceReferralUsage
{
    public class DailyDeviceReferralUsageCompute : IDailyDeviceReferralUsageCompute
    {
        private readonly IComputeLogger<DailyDeviceReferralUsageCompute> _logger;
        private readonly IEventsRepository _repository;

        public DailyDeviceReferralUsageCompute(
            IComputeLogger<DailyDeviceReferralUsageCompute> logger,
            IEventsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task Execute(DateTime startDateTime, DateTime endDateTime)
        {
            _logger.Started(nameof(DailyDeviceReferralUsageCompute), startDateTime, endDateTime);

            startDateTime = DateTime.SpecifyKind(startDateTime, DateTimeKind.Utc);
            endDateTime = DateTime.SpecifyKind(endDateTime, DateTimeKind.Utc);

            await _repository.CallStoredProcedure("CALL compute.devicereferralcompute({0},{1})", startDateTime, endDateTime);
        }
    }

    public interface IDailyDeviceReferralUsageCompute
    {
        public Task Execute(DateTime startDateTime, DateTime endDateTime);
    }
}