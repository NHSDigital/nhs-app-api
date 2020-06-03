using NHSOnline.Backend.Auth.CitizenId.Models;

namespace NHSOnline.Backend.Auth.AspNet
{
    public interface IAccessTokenProvider
    {
        AccessToken AccessToken { get; }
    }
}