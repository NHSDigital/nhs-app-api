namespace NHSOnline.App.Api.Client.Session
{
    internal interface IApiCreateSessionResultVisitor<T>
    {
        T Visit(ApiCreateSessionResult.Success success);
        T Visit(ApiCreateSessionResult.Failure failure);
        T Visit(ApiCreateSessionResult.BadRequest badRequest);
        T Visit(ApiCreateSessionResult.OdsCodeNotSupported odsCodeNotSupported);
        T Visit(ApiCreateSessionResult.OdsCodeNotFound odsCodeNotFound);
        T Visit(ApiCreateSessionResult.NoNhsNumber noNhsNumber);
        T Visit(ApiCreateSessionResult.FailedAgeRequirement failedAgeRequirement);
        T Visit(ApiCreateSessionResult.BadResponseFromUpstreamSystem badResponseFromUpstreamSystem);
        T Visit(ApiCreateSessionResult.UpstreamSystemTimeout odsCodeNotSupportedOrNoNhsNumber);
        T Visit(ApiCreateSessionResult.InternalServerError internalServerError);
    }
}