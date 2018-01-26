namespace NHSOnline.Backend.Worker.Suppliers.Emis
{
    public class EmisSystemProvider : ISystemProvider
    {
        private readonly IEmisClient _emisClient;

        public EmisSystemProvider(IEmisClient emisClient)
        {
            _emisClient = emisClient;
        }

        public INhsNumberProvider GetNhsNumberProvider()
        {
            return new EmisNhsNumberProvider(_emisClient);
        }
    }
}
