using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.CitizenId.Models;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public sealed class NhsLoginGetUserProfileDefaultBehaviour : INhsLoginGetUserProfileBehaviour
    {
        public IActionResult Behave(Patient patient)
        {
            var userInfo = new UserInfo(patient);
            return new JsonResult(userInfo);
        }
    }
}