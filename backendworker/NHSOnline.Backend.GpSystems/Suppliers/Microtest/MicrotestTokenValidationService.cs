namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestTokenValidationService: ITokenValidationService
    {
        public bool IsValidConnectionTokenFormat(string connectionToken)
        {
            // TODO
            // Validate when we know connection token format.
            return true;
        }
    }
}
