using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.UserInfo.Areas.UserInfo.Models;

namespace NHSOnline.Backend.UserInfo.Areas.UserInfo
{
    public class GetInfoResultVisitorV2 : IGetInfoResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetInfoResult.Found result)
        {
            return new OkObjectResult(new UserInfoResponseV2
            {
                Users = result.UserInfoRecords.Select(x => new InfoUserV2
                {
                    NhsLoginId = x.NhsLoginId,
                    NhsNumber = x.Info.NhsNumber,
                    OdsCode = x.Info.OdsCode,
                    LastLogin = x.Timestamp
                })
            });
        }

        public IActionResult Visit(GetInfoResult.NotFound result)
        {
            return new OkObjectResult(new List<InfoUserV2>());
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
