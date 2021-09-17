using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.App.Config;

namespace NHSOnline.App.NhsLogin
{
    internal interface INhsLoginUriBuilderClientIdSetter
    {
        INhsLoginUriBuilderScopeSetter ClientId(string clientId);
    }

    internal interface INhsLoginUriBuilderScopeSetter
    {
        INhsLoginUriBuilderVectorsOfTrustSetter Scopes(NhsLoginScope scope);
    }

    internal interface INhsLoginUriBuilderVectorsOfTrustSetter
    {
        INhsLoginUriBuilderRedirectUriSetter VectorsOfTrust(NhsLoginVectorsOfTrust vectorsOfTrust);
    }

    internal interface INhsLoginUriBuilderRedirectUriSetter : INhsLoginUriBuilder
    {
        INhsLoginUriBuilder RedirectUri(Uri redirectUri);
    }

    internal interface INhsLoginUriBuilder
    {
        INhsLoginUriBuilder Challenge(string challenge, string method);
        INhsLoginUriBuilder FidoAuthResponse(string? fidoAuthResponse);
        INhsLoginUriBuilder AssertedLoginIdentity(string token);
        INhsLoginUriBuilder State(string state);

        Uri Build();
    }

    internal class NhsLoginUriBuilder :
        INhsLoginUriBuilderClientIdSetter,
        INhsLoginUriBuilderScopeSetter,
        INhsLoginUriBuilderVectorsOfTrustSetter,
        INhsLoginUriBuilderRedirectUriSetter
    {
        private static readonly Dictionary<NhsLoginScope, string> ScopesRegister = new
            Dictionary<NhsLoginScope, string>
            {
                { NhsLoginScope.P5NoGpSession, "openid profile profile_extended gp_registration_details" },
                { NhsLoginScope.P9WithGpSession, "openid profile profile_extended nhs_app_credentials gp_registration_details" },
                { NhsLoginScope.P5ToP9Uplift, "openid profile email profile_extended gp_registration_details" },
            };

        private static readonly Dictionary<NhsLoginVectorsOfTrust, string> VectorsOfTrustRegister = new
            Dictionary<NhsLoginVectorsOfTrust, string>
            {
                { NhsLoginVectorsOfTrust.P5Basic, "\"P5.Cp.Cd\", \"P5.Cp.Ck\", \"P5.Cm\"" },
                { NhsLoginVectorsOfTrust.P9Sensitive, "\"P9.Cp.Cd\", \"P9.Cp.Ck\", \"P9.Cm\"" },
                { NhsLoginVectorsOfTrust.P5BasicAndP9Sensitive, "\"P5.Cp.Cd\", \"P5.Cp.Ck\", \"P5.Cm\", \"P9.Cp.Cd\", \"P9.Cp.Ck\", \"P9.Cm\"" },
            };

        private readonly UriBuilder _uriBuilder;
        private readonly Dictionary<string, string> _queryString;

        private NhsLoginUriBuilder(INhsLoginConfiguration config)
        {
            _uriBuilder = new UriBuilder(config.AuthBaseAddress);

            _queryString = new Dictionary<string, string>
            {
                { "state", "A" },
                { "response_type", "code" }
            };
        }

        public static INhsLoginUriBuilderClientIdSetter Create(INhsLoginConfiguration config)
        {
            return new NhsLoginUriBuilder(config);
        }

        public INhsLoginUriBuilderScopeSetter ClientId(string clientId)
        {
            _queryString.Add("client_id", clientId);
            return this;
        }

        public INhsLoginUriBuilderVectorsOfTrustSetter Scopes(NhsLoginScope scope)
        {
            _queryString.Add("scope", ScopesRegister[scope]);
            return this;
        }

        public INhsLoginUriBuilderRedirectUriSetter VectorsOfTrust(NhsLoginVectorsOfTrust vectorsOfTrust)
        {
            _queryString.Add("vtr", $"[{VectorsOfTrustRegister[vectorsOfTrust]}]");
            return this;
        }

        public INhsLoginUriBuilder RedirectUri(Uri redirectUri)
        {
            _queryString.Add("redirect_uri", redirectUri.ToString());
            return this;
        }

        public INhsLoginUriBuilder FidoAuthResponse(string? fidoAuthResponse)
        {
            if (fidoAuthResponse != null)
            {
                _queryString.Add("fido_auth_response", fidoAuthResponse);
            }

            return this;
        }

        public INhsLoginUriBuilder Challenge(string challenge, string method)
        {
            _queryString.Add("code_challenge", challenge);
            _queryString.Add("code_challenge_method", method);
            return this;
        }

        public INhsLoginUriBuilder AssertedLoginIdentity(string token)
        {
            _queryString.Add("asserted_login_identity", token);
            return this;
        }

        public INhsLoginUriBuilder State(string state)
        {
            _queryString["state"] = state;
            return this;
        }

        public Uri Build()
        {
            var queryStringParts =
                _queryString.Select(kvp => $"{Uri.EscapeUriString(kvp.Key)}={Uri.EscapeUriString(kvp.Value)}");
            var queryString = string.Join("&", queryStringParts);

            _uriBuilder.Query = $"?{queryString}";

            return _uriBuilder.Uri;
        }
    }
}