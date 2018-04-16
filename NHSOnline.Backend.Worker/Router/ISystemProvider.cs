using NHSOnline.Backend.Worker.Router.Im1Connection;
using NHSOnline.Backend.Worker.Router.Session;

namespace NHSOnline.Backend.Worker.Router
{
    public interface ISystemProvider
    {
        IIm1ConnectionService GetIm1ConnectionService();
        ISessionService GetSessionService();
        ITokenValidationService GetTokenValidationService();
    }
}
