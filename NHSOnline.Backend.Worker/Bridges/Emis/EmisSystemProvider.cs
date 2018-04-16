using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Im1Connection;
using NHSOnline.Backend.Worker.Router.Session;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisSystemProvider : ISystemProvider
    {
        private readonly IEmisClient _emisClient;

        public EmisSystemProvider(IEmisClient emisClient)
        {
            _emisClient = emisClient;
        }

        public IIm1ConnectionService GetIm1ConnectionService()
        {
            return new EmisIm1ConnectionService(_emisClient);
        }

        public ISessionService GetSessionService()
        {
            // TODO - NHSO-456
            throw new System.NotImplementedException();
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return new EmisTokenValidationService();
        }
    }
}
