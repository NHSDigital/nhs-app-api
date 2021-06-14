namespace NHSOnline.App.Api.Configuration
{
    public interface IGetConfigurationResultVisitor<T>
    {
        T Visit(GetConfigurationResult.Success success);
        T Visit(GetConfigurationResult.Failed failed);
        T Visit(GetConfigurationResult.BadRequest badRequest);
    }
}