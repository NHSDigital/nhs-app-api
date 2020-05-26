using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserResearch
{
    public class PostUserResearchResultVisitor : IUserResearchResultVisitor<IActionResult>
    {
        public IActionResult Visit(PostUserResearchResult.Success result)
        {
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