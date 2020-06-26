using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.App.Config;

namespace NHSOnline.App.NhsLogin
{
    internal interface INhsLoginUriBuilderChallengeSetter
    {
        INhsLoginUriBuilderClientIdSetter Challenge(string challenge, string method);
    }

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
        INhsLoginUriBuilderRedirectUriSetter VectorsOfTrust(string firstVectorOfTrust,
            params string[] otherVectorsOfTrust);
    }

    internal interface INhsLoginUriBuilderRedirectUriSetter
    {
        INhsLoginFidoAuthBuilder RedirectUri(Uri redirectUri);
    }

    internal interface INhsLoginFidoAuthBuilder : INhsLoginUriBuilder
    {
        INhsLoginUriBuilder FidoAuthResponse(string fidoAuthResponse);
    }

    internal interface INhsLoginUriBuilder
    {
        Uri Uri { get; }
    }


    internal class NhsLoginUriBuilder :
        INhsLoginUriBuilderChallengeSetter,
        INhsLoginUriBuilderClientIdSetter,
        INhsLoginUriBuilderScopeSetter,
        INhsLoginUriBuilderVectorsOfTrustSetter,
        INhsLoginUriBuilderRedirectUriSetter,
        INhsLoginFidoAuthBuilder,
        INhsLoginUriBuilder
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

        public static INhsLoginUriBuilderChallengeSetter Create(INhsLoginConfiguration config)
        {
            return new NhsLoginUriBuilder(config);
        }

        public INhsLoginUriBuilderClientIdSetter Challenge(string challenge, string method)
        {
            _queryString.Add("code_challenge", challenge);
            _queryString.Add("code_challenge_method", method);
            return this;
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

        public INhsLoginUriBuilderRedirectUriSetter VectorsOfTrust(string firstVectorOfTrust,
            params string[] otherVectorsOfTrust)
        {
            var quotedValues = string.Join(", ", otherVectorsOfTrust.Prepend(firstVectorOfTrust)
                .Select(x => $"\"{x}\""));
            _queryString.Add("vtr", $"[{quotedValues}]");
            return this;
        }

        public INhsLoginFidoAuthBuilder RedirectUri(Uri redirectUri)
        {
            _queryString.Add("redirect_uri", redirectUri.ToString());
            return this;
        }

        public INhsLoginUriBuilder FidoAuthResponse(string fidoAuthResponse)
        {
            _queryString.Add("fido_auth_response", fidoAuthResponse);
            return this;
        }

        public Uri Uri
        {
            get
            {
                var queryStringParts = _queryString.Select(kvp =>
                    $"{Uri.EscapeUriString(kvp.Key)}={Uri.EscapeUriString(kvp.Value)}");
                var queryString = string.Join("&", queryStringParts);

                _uriBuilder.Query = $"?{queryString}";

                return _uriBuilder.Uri;
            }
        }
    }
}
