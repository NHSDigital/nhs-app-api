namespace NHSOnline.App.Api.Client.Session
{
    internal interface IApiCreateSessionResultVisitor<T>
    {
        T Visit(ApiCreateSessionResult.Success success);
        T Visit(ApiCreateSessionResult.Failure failure);
        T Visit(ApiCreateSessionResult.BadRequest badRequest);
        T Visit(ApiCreateSessionResult.Forbidden forbidden);
        T Visit(ApiCreateSessionResult.OdsCodeNotSupportedOrNoNhsNumber odsCodeNotSupportedOrNoNhsNumber);
    }
}