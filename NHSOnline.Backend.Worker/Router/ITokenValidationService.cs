namespace NHSOnline.Backend.Worker.Router
{
    public interface ITokenValidationService
    {
        bool IsValidConnectionTokenFormat(string connectionToken);
    }
}
