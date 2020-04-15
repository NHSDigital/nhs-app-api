using System.Runtime.CompilerServices;

namespace NHSOnline.Backend.Support.Session
{
    public interface IUserSessionService
    {
        TUserSession GetRequiredUserSession<TUserSession>([CallerMemberName] string context = "") where TUserSession: UserSession;
        Option<TUserSession> GetUserSession<TUserSession>() where TUserSession : UserSession;
    }
}
