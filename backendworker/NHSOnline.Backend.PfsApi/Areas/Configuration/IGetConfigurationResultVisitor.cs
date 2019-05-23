namespace NHSOnline.Backend.PfsApi.Areas.Configuration
{
    public interface IGetConfigurationResultVisitor<out T>
    {
        T Visit(GetConfigurationResult.Success result);

        T Visit(GetConfigurationResult.BadRequest result);

        T Visit(GetConfigurationResult.InternalServerError result);       
    }
}