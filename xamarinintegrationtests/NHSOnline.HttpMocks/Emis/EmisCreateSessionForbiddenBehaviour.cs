using System.Net;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public sealed class EmisCreateSessionForbiddenBehaviour : IEmisCreateSessionBehaviour
    {
        public IActionResult Behave(EmisPatient patient) => new StatusCodeResult((int)HttpStatusCode.Forbidden);
    }
}