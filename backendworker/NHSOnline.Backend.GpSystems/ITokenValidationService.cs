namespace NHSOnline.Backend.GpSystems
{
    public interface ITokenValidationService
    {
        bool IsValidConnectionTokenFormat(string connectionToken);
    }
}
