using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public class GetMeInfoResultVisitor : IGetInfoResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetInfoResult.Found result)
        {
            return new OkObjectResult(result.UserInfoRecords.FirstOrDefault());
        }

        public IActionResult Visit(GetInfoResult.NotFound result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(GetInfoResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetInfoResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}