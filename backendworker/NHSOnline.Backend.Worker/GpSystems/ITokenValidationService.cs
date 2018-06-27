namespace NHSOnline.Backend.Worker.GpSystems
{
    public interface ITokenValidationService
    {
        bool IsValidConnectionTokenFormat(string connectionToken);
    }
}
