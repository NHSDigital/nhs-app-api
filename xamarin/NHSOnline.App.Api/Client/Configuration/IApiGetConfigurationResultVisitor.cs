namespace NHSOnline.App.Api.Client.Configuration
{
    internal interface IApiGetConfigurationResultVisitor<T>
    {
        T Visit(ApiGetConfigurationResult.Success success);
        T Visit(ApiGetConfigurationResult.Failure failure);
        T Visit(ApiGetConfigurationResult.BadRequest badRequest);
    }
}