using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Ndop
{
    [Route("ndop")]
    public class NdopController : Controller
    {
        private readonly IPatients _patients;

        public NdopController(IPatients patients)
        {
            _patients = patients;
        }

        [HttpPost]
        [Route("createsession")]
        public IActionResult CreateSession([FromForm] string token)
        {

            var tokenContent = GetTokenContent(token);

            var patient = _patients.LookupByNhsNumber(tokenContent["nhs_number"]);
            if (patient == null)
            {
                return Unauthorized();
            }

            var behaviour = patient.Behaviours.Get<INdopCreateSessionBehaviour>(() => new NdopCreateSessionDefaultBehaviour());
            return behaviour.Behave(token, tokenContent, HttpContext.Request, View);
        }

        private Dictionary<string, string> GetTokenContent(string token)
        {
            var tokenContent = new Dictionary<string, string>();
            var handler = new JwtSecurityTokenHandler();
            var tokenValue = handler.ReadJwtToken(token);

            tokenContent.Add(nameof(tokenValue.Audiences), tokenValue.Audiences.First());
            tokenContent.Add(nameof(tokenValue.Issuer), tokenValue.Issuer);
            foreach (var claim in tokenValue.Claims)
            {
                tokenContent.Add(claim.Type, claim.Value);
            }

            return tokenContent;
        }
    }
}