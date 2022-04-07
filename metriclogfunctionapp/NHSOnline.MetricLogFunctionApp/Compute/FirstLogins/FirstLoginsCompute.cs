using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Compute.FirstLogins
{
    public class FirstLoginsCompute : IFirstLoginsCompute
    {
        private readonly IComputeLogger<FirstLoginsCompute> _logger;
        private readonly IEventsRepository _repository;

        public FirstLoginsCompute(
            IComputeLogger<FirstLoginsCompute> logger,
            IEventsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task Execute(string loginId, DateTime startDateTime, DateTime endDateTime)
        {
            _logger.Started(nameof(FirstLoginsCompute), startDateTime,endDateTime);

            startDateTime = DateTime.SpecifyKind(startDateTime, DateTimeKind.Utc);
            endDateTime = DateTime.SpecifyKind(endDateTime, DateTimeKind.Utc);

            await _repository.CallStoredProcedure("CALL compute.FirstLoginsComputation({0},{1},{2})", loginId, startDateTime, endDateTime);
        }
    }

    public interface IFirstLoginsCompute
    {
        public Task Execute(string loginId, DateTime startDateTime, DateTime endDateTime);
    }
}