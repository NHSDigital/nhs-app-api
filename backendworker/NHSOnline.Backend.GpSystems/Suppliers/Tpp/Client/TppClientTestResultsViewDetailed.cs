using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientTestResultsViewDetailed
        : ITppClientRequest<(TppRequestParameters tppRequestParameters, string testResultId), TestResultsViewReply>
    {
        private readonly ILogger<TppClientTestResultsViewDetailed> _logger;
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientTestResultsViewDetailed(
            ILogger<TppClientTestResultsViewDetailed> logger,
            TppClientRequestExecutor requestExecutor)
        {
            _logger = logger;
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<TestResultsViewReply>> Post(
            (TppRequestParameters tppRequestParameters, string testResultId) parameters)
        {
            _logger.LogEnter();

            var (tppRequestParameters, testResultId) = parameters;

            try
            {
                var request = new TestResultsViewDetailed
                {
                    PatientId = tppRequestParameters.PatientId,
                    OnlineUserId = tppRequestParameters.OnlineUserId,
                    UnitId = tppRequestParameters.OdsCode,
                    TestResultId = testResultId,
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