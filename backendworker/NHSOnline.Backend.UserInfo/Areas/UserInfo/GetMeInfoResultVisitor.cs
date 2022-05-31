using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UserInfo.Areas.UserInfo.Models;

namespace NHSOnline.Backend.UserInfo.Areas.UserInfo
{
    public class GetMeInfoResultVisitor : IGetInfoResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetInfoResult.Found result)
        {
            return new OkObjectResult(result.UserInfoRecords.Select(x => new InfoUserV1
                {
                    NhsLoginId = x.NhsLoginId,
                    Info = new InfoV1
                    {
                        NhsNumber = x.Info?.NhsNumber,
                        OdsCode = x.Info?.OdsCode
                    },
                    Timestamp = x.Timestamp
                }).FirstOrDefault()
            );
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