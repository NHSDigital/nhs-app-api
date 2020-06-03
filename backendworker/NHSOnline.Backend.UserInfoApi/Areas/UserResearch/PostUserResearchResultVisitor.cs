using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Metrics;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserResearch
{
    public class PostUserResearchResultVisitor : IUserResearchResultVisitor<IActionResult>
    {
        private readonly IMetricLogger _metricLogger;

        public PostUserResearchResultVisitor(IMetricLogger metricLogger)
        {
            _metricLogger = metricLogger;
        }
        public IActionResult Visit(PostUserResearchResult.Success result)
        {
            _metricLogger.UserResearchOptIn();
            return new NoContentResult();
        }

        public IActionResult Visit(PostUserResearchResult.Failure result)
        {
           return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(PostUserResearchResult.EmailMissing result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(PostUserResearchResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}