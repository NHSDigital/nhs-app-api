using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    [Route("citizenid")]
    public class CitizenIdController : Controller
    {
        private const string AuthHostName = "auth.nhslogin.stubs.local.bitraft.io";
        internal const string CidBase = "http://" + AuthHostName + ":8080/citizenid/";
        internal const string Base64EncodedJwtCertificatePfx = "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";

        private readonly ILogger<CitizenIdController> _logger;
        private readonly IPatients _patients;

        public CitizenIdController(
            ILogger<CitizenIdController> logger,
            IPatients patients)
        {
            _logger = logger;
            _patients = patients;
        }

        [Host(AuthHostName)]
        [HttpGet("authorize")]
        public IActionResult LoginScreen(
            [FromQuery(Name = "redirect_uri")] string redirect,
            string state)
        {
            return Content($@"
                <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1, shrink-to-fit=no'>
                        <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css' integrity='sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk' crossorigin='anonymous'>
                        <title>NHS Login Stubbed</title>
                    </head>
                    <body>
                        <div class='container'>
                            <h1 class='display-4'>NHS Login</h1>
                            <ul>
                                <li><a href='javascript: window.history.back();'>Back</a></li>
                                <li><a href='http://{AuthHostName}:8080/citizenid/Page/Internal.html'>Internal Page</a></li>
                                <li><a href='http://{AuthHostName}:8080/citizenid/Page/Internal.html' target='_blank'>Internal Page (New Window)</a></li>
                                <li><a href='http://stubs.local.bitraft.io:8080/nhsuk/covid'>Covid</a></li>
                            </ul>
                            <hr>
                            <form action='complete-login' method='get'>
                                <input value='{redirect}' type='hidden' name='redirect_uri'>
                                <input value='{state}' type='hidden' name='state'>
                                <div class='form-group'>
                                    <label for='PatientId'>Patient ID</label>
                                    <input class='form-control placeholder='Patient ID' type='text' name='patientId' id='PatientId'>
                                </div>
                                <input type='submit' value='Login'>
                            </form>
                        </div>
                    </body>
                </html>",
                "text/html");
        }

        [Host(AuthHostName)]
        [HttpGet("Page/Internal.html")]
        public IActionResult InternalPage()
        {
            return Content($@"
                <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1, shrink-to-fit=no'>
                        <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css' integrity='sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk' crossorigin='anonymous'>
                        <title>NHS Login Stubbed - Internal Page</title>
                    </head>
                    <body>
                        <div class='container'>
                            <h1 class='display-4'>NHS Login - Internal Page</h1>
                            <ul>
                                <li><a href='javascript: window.history.back();'>Back</a></li>
                                <li><a href='http://{AuthHostName}:8080/citizenid/Page/Other.html'>Other Page</a></li>
                                <li><a href='http://{AuthHostName}:8080/citizenid/Page/Other.html' target='_blank'>Other Page (New Window)</a></li>
                                <li><a href='http://stubs.local.bitraft.io:8080/nhsuk/covid'>Covid</a></li>
                            </ul>
                        </div>
                    </body>
                </html>",
                "text/html");
        }

        [Host(AuthHostName)]
        [HttpGet("Page/Other.html")]
        public IActionResult OtherPage()
        {
            return Content($@"
                <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1, shrink-to-fit=no'>
                        <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css' integrity='sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk' crossorigin='anonymous'>
                        <title>NHS Login Stubbed - Other Page</title>
                    </head>
                    <body>
                        <div class='container'>
                            <h1 class='display-4'>NHS Login - Other Page</h1>
                            <ul>
                                <li><a href='javascript: window.history.back();'>Back</a></li>
                                <li><a href='http://{AuthHostName}:8080/citizenid/Page/Internal.html'>Internal Page</a></li>
                                <li><a href='http://{AuthHostName}:8080/citizenid/Page/Internal.html' target='_blank'>Internal Page (New Window)</a></li>
                                <li><a href='http://stubs.local.bitraft.io:8080/nhsuk/covid'>Covid</a></li>
                            </ul>
                        </div>
                    </body>
                </html>",
                "text/html");
        }

        [Host(AuthHostName)]
        [HttpGet("complete-login")]
        public async Task<IActionResult> CompleteLogin(
            [FromQuery(Name = "redirect_uri")] string redirect,
            [FromQuery] string state,
            [FromQuery] string patientId)
        {
            var patient = _patients.LookupById(patientId);
            if (patient == null)
            {
                return Unauthorized();
            }

            var behaviour = patient.Behaviours.Get<INhsLoginAuthoriseBehaviour>(() => new NhsLoginAuthoriseDefaultBehaviour());

            return await behaviour.Behave(patient, redirect, state);
        }

        [Host(AuthHostName)]
        [HttpPost("token")]
        public IActionResult Token(string code)
        {
            var patient = _patients.LookupById(code);
            if (patient == null)
            {
                return NotFound();
            }

            var behaviour = patient.Behaviours.Get<INhsLoginTokenBehaviour>(() => new NhsLoginTokenDefaultBehaviour());

            return behaviour.Behave(patient);
        }

        [Host(AuthHostName)]
        [HttpGet(".well-known/jwks.json")]
        public IActionResult TokenCertificate()
        {
            using var certificate = new X509Certificate2(Convert.FromBase64String(Base64EncodedJwtCertificatePfx), "perftest");
            var key = new X509SecurityKey(certificate);
            var jsonWebKey = JsonWebKeyConverter.ConvertFromX509SecurityKey(key);
            var jsonWebKeySet = new JsonWebKeySet { Keys = { jsonWebKey } };
            return Json(jsonWebKeySet);
        }

        [Host(AuthHostName)]
        [HttpGet("userinfo")]
        public IActionResult GetUserProfile()
        {
            var accessToken = Request.Headers["Authorization"].Single().Split(' ')[1];
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);
            var patientName = token.Subject;

            _logger.LogInformation($"token subject is {patientName}");

            var patient = _patients.LookupById(patientName);
            if (patient == null)
            {
                return NotFound();
            }

            var behaviour = patient.Behaviours.Get<INhsLoginGetUserProfileBehaviour>(() => new NhsLoginGetUserProfileDefaultBehaviour());

            return behaviour.Behave(patient);

        }

        [Host(AuthHostName)]
        [Route(".well-known/openid-configuration")]
        public IActionResult GetOpenIdConfig()
        {
            return Json(new
            {
                issuer = $"{CidBase}",
                authorization_endpoint = $"{CidBase}authorize",
                token_endpoint = $"{CidBase}token",
                jwks_uri = $"{CidBase}.well-known/jwks.json",
                scopes_supported = new[]
                {
                    "address",
                    "email",
                    "gp_integration_credentials",
                    "gp_registration_details",
                    "nhs_app_credentials",
                    "openid",
                    "phone",
                    "profile",
                    "profile_extended"
                },
                response_types_supported = new[]
                {
                    "code"
                },
                subject_types_supported = new[]
                {
                    "public"
                },
                id_token_signing_alg_values_supported = new[]
                {
                    "RS512"
                },
                claims_supported = new[]
                {
                    "iss",
                    "aud",
                    "sub",
                    "family_name",
                    "given_name",
                    "email",
                    "email_verified",
                    "phone_number",
                    "phone_number_verified",
                    "birthdate",
                    "address",
                    "nhs_number",
                    "gp_integration_credentials",
                    "delegations",
                    "gp_registration_details",
                    "im1_token",
                    "auth_time",
                    "jti",
                    "nonce",
                    "vot",
                    "vtm",
                    "exp",
                    "ods_code",
                    "user_id",
                    "linkage_key",
                    "surname",
                    "iat",
                    "gp_ods_code",
                    "gp_user_id",
                    "gp_linkage_key"
                },
                userinfo_endpoint = $"{CidBase}userinfo",
                token_endpoint_auth_signing_alg_values_supported = new[]
                {
                    "RS512"
                },
                token_endpoint_auth_methods_supported = new[]
                {
                    "private_key_jwt"
                },
                fido_uaf_authentication_request_endpoint = $"{CidBase}authRequest",
                fido_uaf_registration_request_endpoint = $"{CidBase}regRequest",
                fido_uaf_registration_response_endpoint = $"{CidBase}regResponse",
                fido_uaf_deregistration_endpoint = $"{CidBase}dereg"
            }
          );
        }
    }
}
