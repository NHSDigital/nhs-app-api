using NHSOnline.Backend.Worker.Router.Im1Connection;

namespace NHSOnline.Backend.Worker.Router
{
    public interface ISystemProvider
    {
        IIm1ConnectionService GetIm1ConnectionService();
        ITokenValidationService GetTokenValidationService();
    }
}
