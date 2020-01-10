namespace NHSOnline.Backend.PfsApi.Areas.Configuration
{
    public interface IGetConfigurationResultVisitorV2<out T>
    {
        T Visit(GetConfigurationResultV2.Success result);

        T Visit(GetConfigurationResultV2.InternalServerError result);       
    }
}