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
        INhsLoginUriBuilderVectorsOfTrustSetter Scopes(string firstScope, params string[] otherScopes);
    }

    internal interface INhsLoginUriBuilderVectorsOfTrustSetter
    {
        INhsLoginUriBuilderRedirectUriSetter VectorsOfTrust(string firstVectorOfTrust, params string[] otherVectorsOfTrust);
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
        private readonly UriBuilder _uriBuilder;
        private readonly Dictionary<string, string> _queryString;

        private NhsLoginUriBuilder(INhsLoginConfiguration config)
        {
            _uriBuilder = new UriBuilder(config.AuthBaseAddress);

            _queryString = new Dictionary<string, string>
            {
                {"state", "A"},
                {"response_type", "code"}
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

        public INhsLoginUriBuilderVectorsOfTrustSetter Scopes(string firstScope, params string[] otherScopes)
        {
            _queryString.Add("scope", string.Join(" ", otherScopes.Prepend(firstScope)));
            return this;
        }

        public INhsLoginUriBuilderRedirectUriSetter VectorsOfTrust(string requiredFirstVectorOfTrust,
            params string[] otherVectorsOfTrust)
        {
            var quotedValues = string.Join(", ", otherVectorsOfTrust.Prepend(requiredFirstVectorOfTrust)
                .Select(x => $"\"{x}\""));
            _queryString.Add("vtr", $"[{quotedValues}]");
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
            var queryStringParts = _queryString.Select(kvp => $"{Uri.EscapeUriString(kvp.Key)}={Uri.EscapeUriString(kvp.Value)}");
            var queryString = string.Join("&", queryStringParts);

            _uriBuilder.Query = $"?{queryString}";

            return _uriBuilder.Uri;
        }
    }
}
