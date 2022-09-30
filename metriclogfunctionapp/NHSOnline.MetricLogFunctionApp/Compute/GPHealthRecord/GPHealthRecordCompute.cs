using System;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Compute.GPHealthRecord
{
    public interface IGPHealthRecordCompute
    {
        public Task Execute(DateTime startDateTime, DateTime endDateTime);
    }

    public class GPHealthRecordCompute : IGPHealthRecordCompute
    {
        private readonly IComputeLogger<GPHealthRecordCompute> _logger;
        private readonly IEventsRepository _repository;

        public GPHealthRecordCompute(
            IComputeLogger<GPHealthRecordCompute> logger,
            IEventsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task Execute(DateTime startDateTime, DateTime endDateTime)
        {
            _logger.Started(nameof(GPHealthRecordCompute), startDateTime, endDateTime);
            await _repository.CallStoredProcedure("CALL compute.GPRecordViewsComputation({0},{1})",
                startDateTime, endDateTime);
        }
    }
}
