using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NHSOnline.Backend.GpSystems.SessionManager.Model;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public interface ISessionMapper
    {
        P9UserSession Map(StringValues csrfToken, GpUserSession gpUserSession, GpSessionManagerCitizenIdUserSession citizenIdUserSession, string im1ConnectionToken);
    }
}
