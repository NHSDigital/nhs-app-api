using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.CitizenId
{
    public sealed class NhsLoginTokenDefaultBehaviour : INhsLoginTokenBehaviour
    {
        public IActionResult Behave(Patient patient)
        {
            var patientJson = GetToken(patient);

            return patientJson;
        }

        private static JsonResult GetToken(Patient patient)
        {
            var certificateBytes = Convert.FromBase64String(CitizenIdController.Base64EncodedJwtCertificatePfx);
            using var certificate = new X509Certificate2(certificateBytes, "perftest");

            var key = new X509SecurityKey(certificate);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha512);

            var jwtHeader = new JwtHeader(signingCredentials);

            var idToken = BuildIdToken(jwtHeader, patient);
            var accessToken = BuildAccessToken(jwtHeader, patient);

            return new JsonResult(new
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = "90",
                scope = "openid profile nhs_app_credentials gp_integration_credentials",
                id_token = idToken
            });
        }

        private static string BuildIdToken(JwtHeader jwtHeader, Patient patient)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", patient.Id),
                new Claim("email_verified", "true"),
                new Claim("nhs_number", patient.NhsNumber.FormattedStringValue),
                new Claim("id_status", "verified"),
                new Claim("token_use", "id"),
                new Claim("surname", patient.PersonalDetails.Name.FamilyName),
                new Claim("email", patient.PersonalDetails.ContactDetails.EmailAddress),
                new Claim("birthdate", patient.PersonalDetails.Age.DateOfBirthISO86012004),
                new Claim("jti", Guid.NewGuid().ToString())
            };

            if (patient is IGpRegistered registered)
            {
                claims.Add(new Claim("im1_token", registered.Im1ConnectionToken));
                claims.Add(new Claim("ods_code", registered.OdsCode));
            }

            var token = BuildToken(jwtHeader, claims);

            return token;
        }

        private static string BuildAccessToken(JwtHeader jwtHeader, Patient patient)
        {
            var authTime = EpochTime.GetIntDate(DateTime.UtcNow);
            var expiryTime = authTime + 3600;

            var claims = new[]
            {
                new Claim("sub", patient.Id),
                new Claim("nhs_number", patient.NhsNumber.FormattedStringValue),
                new Claim("iss", CitizenIdController.CidBase),
                new Claim("version", "0"),
                new Claim("vtm", CitizenIdController.CidBase),
                new Claim("client_id", "nhs-online"),
                new Claim("requesting_patient", patient.NhsNumber.FormattedStringValue),
                new Claim("aud", "nhs-online"),
                new Claim("token_use", "access"),
                new Claim("auth_time", authTime.ToString(CultureInfo.InvariantCulture)),
                new Claim("scope", "openid profile nhs_app_credentials gp_integration_credentials profile_extended"),
                new Claim("vot", patient.VectorOfTrust),
                new Claim("exp", authTime.ToString(CultureInfo.InvariantCulture)),
                new Claim("iat", expiryTime.ToString(CultureInfo.InvariantCulture)),
                new Claim("reason_for_request", "patientaccess"),
                new Claim("jti", Guid.NewGuid().ToString()),
                new Claim("identity_proofing_level", patient.ProofingLevel)
            };

            var token = BuildToken(jwtHeader, claims);

            return token;
        }

        private static string BuildToken(JwtHeader jwtHeader, IEnumerable<Claim> claims)
        {
            var jwtPayload = new JwtPayload(
                issuer: CitizenIdController.CidBase,
                audience: "nhs-online",
                claims: claims,
                notBefore: null,
                expires: DateTime.UtcNow.AddMinutes(30));
            var jwt = new JwtSecurityToken(jwtHeader, jwtPayload);
            var serialisedToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            return serialisedToken;
        }
    }
}