using System;
using System.Globalization;
using NHSOnline.App.Api.Client.Session;

namespace NHSOnline.App.Api.Session
{
    public sealed class UserSession
    {
        private readonly UserSessionResponse _response;

        internal UserSession(UserSessionResponse response)
        {
            _response = response;
        }

        public string? Name => _response.Name;

        public int SessionTimeout => _response.SessionTimeout;

        public string? OdsCode => _response.OdsCode;

        public string? Token  => _response.Token;

        public string LastCalledAt { get; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        public string? NhsNumber  => _response.NhsNumber;

        public string? DateOfBirth  => _response.DateOfBirth;

        public string? AccessToken  => _response.AccessToken;

        public string? ProofLevel  => _response.ProofLevel;
    }
}