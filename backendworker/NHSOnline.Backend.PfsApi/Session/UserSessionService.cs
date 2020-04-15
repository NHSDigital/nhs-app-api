using System;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public sealed class UserSessionService: IUserSessionService
    {
        private UserSession _userSession;

        internal void SetUserSession(UserSession userSession) => _userSession = userSession;

        TUserSession IUserSessionService.GetRequiredUserSession<TUserSession>(string context)
        {
            if (_userSession is TUserSession userSession)
            {
                return userSession;
            }

            throw new InvalidOperationException(
                $"{context}: Required {typeof(TUserSession).Name} session but current session is {_userSession?.GetType().Name ?? "null"}");
        }

        Option<TUserSession> IUserSessionService.GetUserSession<TUserSession>()
        {
            if (_userSession is TUserSession userSession)
            {
                return Option.Some(userSession);
            }

            return Option.None<TUserSession>();
        }
    }
}