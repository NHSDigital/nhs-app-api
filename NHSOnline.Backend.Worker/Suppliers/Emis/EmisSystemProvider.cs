namespace NHSOnline.Backend.Worker.Suppliers.Emis
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
    }
}