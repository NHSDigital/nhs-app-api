using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientTestResultsView
        : ITppClientRequest<(TppRequestParameters tppRequestParameters, string startDate, string endDate), TestResultsViewReply>
    {
        private readonly ILogger<TppClientTestResultsView> _logger;
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientTestResultsView(ILogger<TppClientTestResultsView> logger, TppClientRequestExecutor requestExecutor)
        {
            _logger = logger;
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<TestResultsViewReply>> Post(
            (TppRequestParameters tppRequestParameters, string startDate, string endDate) parameters)
        {
            var (tppRequestParameters, startDate, endDate) = parameters;

            _logger.LogDebug($"Entered: {nameof(TestResultsView)} with { nameof(startDate)}:{startDate} and {nameof(endDate)}:{endDate}");

            try
            {
                var request = new TestResultsView
                {
                    PatientId = tppRequestParameters.PatientId,
                    OnlineUserId = tppRequestParameters.OnlineUserId,
                    UnitId = tppRequestParameters.OdsCode,
                    StartDate = startDate,
                    EndDate = endDate
                };

                return await _requestExecutor.Post<TestResultsViewReply>(
                    requestBuilder => requestBuilder.Model(request).Suid(tppRequestParameters.Suid));
            }
            finally
            {
                _logger.LogExit();
            }

        }
    }
}