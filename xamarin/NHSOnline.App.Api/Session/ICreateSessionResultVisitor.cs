namespace NHSOnline.App.Api.Session
{
    public interface ICreateSessionResultVisitor<T>
    {
        T Visit(CreateSessionResult.Created created);
        T Visit(CreateSessionResult.Failed failed);
        T Visit(CreateSessionResult.BadRequest badRequest);
        T Visit(CreateSessionResult.OdsCodeNotSupported odsCodeNotSupported);
        T Visit(CreateSessionResult.OdsCodeNotFound odsCodeNotFound);
        T Visit(CreateSessionResult.NoNhsNumber noNhsNumber);
        T Visit(CreateSessionResult.FailedAgeRequirement failedAgeRequirement);
        T Visit(CreateSessionResult.BadResponseFromUpstreamSystem badResponseFromUpstreamSystem);
        T Visit(CreateSessionResult.UpstreamSystemTimeout odsCodeNotSupportedOrNoNhsNumber);
        T Visit(CreateSessionResult.InternalServerError internalServerError);
    }
}