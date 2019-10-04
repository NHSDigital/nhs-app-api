using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public class PostInfoResultVisitor : IInfoResultVisitor<IActionResult>
    {
        public IActionResult Visit(PostInfoResult.Created result)
        {
            return new StatusCodeResult(StatusCodes.Status201Created);
        }
        public IActionResult Visit(PostInfoResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        public IActionResult Visit(PostInfoResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
