namespace NHSOnline.Backend.PfsApi.Areas.Configuration
{
    public interface IGetConfigurationResultVisitor<out T>
    {
        T Visit(GetConfigurationResult.SuccessfullyRetrieved result);

        T Visit(GetConfigurationResult.MissingDetailsResult result);

        T Visit(GetConfigurationResult.InvalidNativeAppVersionResult result);

        T Visit(GetConfigurationResult.InvalidDeviceNameResult result);

        T Visit(GetConfigurationResult.ErrorRetrievingConfigResult result);       
    }
}