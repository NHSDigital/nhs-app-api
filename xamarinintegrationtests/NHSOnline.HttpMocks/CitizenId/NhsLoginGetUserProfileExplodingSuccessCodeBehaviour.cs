using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    /// <summary>
    /// As part of the Create Session process, PFS calls the `userinfo` endpoint to get user details.
    /// We can fudge this and make PFS explode by returning a status less than or equal to 299 with no valid Json body.
    /// </summary>
    public sealed class NhsLoginGetUserProfileExplodingSuccessCodeBehaviour : INhsLoginGetUserProfileBehaviour
    {
        public IActionResult Behave(Patient patient)
        {
            return new StatusCodeResult(298);
        }
    }
}