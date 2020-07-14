using System;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.Support.Session;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    internal sealed class CreateResponseFromUserSessionVisitor<TUserSessionResponse> : IUserSessionVisitor<TUserSessionResponse>
        where TUserSessionResponse : UserSessionResponse
    {
        private readonly ConfigurationSettings _settings;
        private readonly TUserSessionResponse _userSessionResponse;

        public CreateResponseFromUserSessionVisitor(
            ConfigurationSettings settings,
            TUserSessionResponse userSessionResponse)
        {
            _settings = settings;
            _userSessionResponse = userSessionResponse;
        }

        public TUserSessionResponse Visit(P5UserSession userSession)
        {
            SetCommonProperties(userSession);
            _userSessionResponse.Im1MessagingEnabled = false;
            return _userSessionResponse;
        }

        public TUserSessionResponse Visit(P9UserSession userSession)
        {
            SetCommonProperties(userSession);
            _userSessionResponse.NhsNumber = userSession.NhsNumber;
            _userSessionResponse.Im1MessagingEnabled = userSession.GpUserSession?.Im1MessagingEnabled ?? false;
            return _userSessionResponse;

        }

        private void SetCommonProperties(P5UserSession userSession)
        {
            _userSessionResponse.SessionTimeout = (int)TimeSpan.FromMinutes(_settings.DefaultSessionExpiryMinutes).TotalSeconds;
            _userSessionResponse.OdsCode = userSession.OdsCode;
            _userSessionResponse.DateOfBirth = userSession.CitizenIdUserSession.DateOfBirth;
            _userSessionResponse.Name = userSession.CitizenIdUserSession.Name;
            _userSessionResponse.Token = userSession.CsrfToken;
            _userSessionResponse.AccessToken = userSession.CitizenIdUserSession.AccessToken;
            _userSessionResponse.ProofLevel = userSession.CitizenIdUserSession.ProofLevel;
        }
    }
}