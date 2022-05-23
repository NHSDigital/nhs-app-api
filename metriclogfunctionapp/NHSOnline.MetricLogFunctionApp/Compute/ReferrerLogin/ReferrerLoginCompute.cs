using System;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Compute.ReferrerLogin
{
    public interface IReferrerLoginCompute
    {
        public Task Execute(DateTime startDateTime, DateTime endDateTime);
    }

    public class ReferrerLoginCompute : IReferrerLoginCompute
    {
        private readonly IComputeLogger<ReferrerLoginCompute> _logger;
        private readonly IEventsRepository _repository;

        public ReferrerLoginCompute(
            IComputeLogger<ReferrerLoginCompute> logger,
            IEventsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task Execute(DateTime startDateTime, DateTime endDateTime)
        {
            _logger.Started(nameof(ReferrerLoginCompute), startDateTime, endDateTime);
            await _repository.CallStoredProcedure("CALL compute.ReferrerLoginComputation({0},{1})", startDateTime, endDateTime);
        }
    }
}
