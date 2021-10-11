using System;
using System.IdentityModel.Tokens.Jwt;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Host(HostName)]
    public class VaccineRecordController : Controller
    {
        public const string HostName = "nhsd.stubs.local.bitraft.io";

        private readonly IPatients _patients;

        public VaccineRecordController(IPatients patients)
        {
            _patients = patients;
        }

        [HttpGet("sso")]
        public IActionResult StartSingleSignOn([FromQuery]string assertedLoginIdentity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(assertedLoginIdentity);
            if (!jwtToken.Payload.TryGetCode(out var code))
            {
                return BadRequest();
            }
            var patient = _patients.LookupById(code);
            if (patient is null)
            {
                return Unauthorized();
            }

            var behaviour = patient.Behaviours.Get<IVaccineRecordSsoBehaviour>(() => new DefaultVaccineRecordSsoBehaviour());

            return behaviour.Behave(patient);
        }

        [HttpGet("vaccine-record")]
        public IActionResult VaccineRecordPage()
        {
            (string Title, HttpRequest Request) model = ("Vaccine Record Internal Page", Request);
            return View("~/Views/WebIntegrations/VaccineRecordPage.cshtml", model);
        }
    }
}