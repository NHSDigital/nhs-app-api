namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeTokenValidationService : ITokenValidationService
    {
        public bool IsValidConnectionTokenFormat(string connectionToken)
        {
            return true;
        }
    }
}