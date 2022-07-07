using System;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Compute.Wayfinder
{
    public class WayfinderCompute : IWayfinderCompute
    {
        private readonly IComputeLogger<WayfinderCompute> _logger;
        private readonly IEventsRepository _repository;

        public WayfinderCompute(
            IComputeLogger<WayfinderCompute> logger,
            IEventsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task Execute(DateTime startDateTime, DateTime endDateTime)
        {
            _logger.Started(nameof(WayfinderCompute), startDateTime, endDateTime);
            await _repository.CallStoredProcedure("CALL compute.WayfinderComputation({0},{1})",
                startDateTime, endDateTime);
        }
    }
}
