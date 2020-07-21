using System.Net;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public sealed class NhsLoginTokenBadGatewayBehaviour : INhsLoginTokenBehaviour
    {
        public IActionResult Behave(Patient patient)
        {
            return new StatusCodeResult((int)HttpStatusCode.BadGateway);
        }
    }
}