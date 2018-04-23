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

        public SupplierEnum Supplier => SupplierEnum.Emis;

        public IIm1ConnectionService GetIm1ConnectionService()
        {
            return new EmisIm1ConnectionService(_emisClient);
        }

        public ISessionService GetSessionService()
        {
            return new EmisSessionService(_emisClient);
        }

        public ITokenValidationService GetTokenValidationService()
        {
            return new EmisTokenValidationService();
        }
    }
}
