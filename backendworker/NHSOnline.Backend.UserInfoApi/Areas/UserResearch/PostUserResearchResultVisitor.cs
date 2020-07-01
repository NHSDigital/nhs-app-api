using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Metrics;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserResearch
{
    public class PostUserResearchResultVisitor : IUserResearchResultVisitor<Task<IActionResult>>
    {
        private readonly IMetricLogger _metricLogger;

        public PostUserResearchResultVisitor(IMetricLogger metricLogger)
        {
            _metricLogger = metricLogger;
        }
        public async Task<IActionResult> Visit(PostUserResearchResult.Success result)
        {
            await _metricLogger.UserResearchOptIn();
            return new NoContentResult();
        }

        public async Task<IActionResult> Visit(PostUserResearchResult.Failure result)
        {
           return await Task.FromResult(new StatusCodeResult(StatusCodes.Status502BadGateway));
        }

        public async Task<IActionResult> Visit(PostUserResearchResult.EmailMissing result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status502BadGateway));
        }

        public async Task<IActionResult> Visit(PostUserResearchResult.InternalServerError result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status500InternalServerError));
        }
    }
}