using System;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Compute.ReferrerServiceJourney
{
    public interface IReferrerServiceJourneyCompute
    {
        public Task Execute(DateTime startDateTime, DateTime endDateTime);
    }

    public class ReferrerServiceJourneyCompute : IReferrerServiceJourneyCompute
    {
        private readonly IComputeLogger<ReferrerServiceJourneyCompute> _logger;
        private readonly IEventsRepository _repository;

        public ReferrerServiceJourneyCompute(
            IComputeLogger<ReferrerServiceJourneyCompute> logger,
            IEventsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task Execute(DateTime startDateTime, DateTime endDateTime)
        {
            _logger.Started(nameof(ReferrerServiceJourneyCompute), startDateTime, endDateTime);
            await _repository.CallStoredProcedure("CALL compute.ReferrerServiceJourneyComputation({0},{1})",
                                                  startDateTime, endDateTime);
        }
    }
}
