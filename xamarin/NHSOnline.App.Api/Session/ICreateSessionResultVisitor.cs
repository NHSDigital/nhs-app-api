namespace NHSOnline.App.Api.Session
{
    public interface ICreateSessionResultVisitor<T>
    {
        T Visit(CreateSessionResult.Created created);
        T Visit(CreateSessionResult.Failed failed);
        T Visit(CreateSessionResult.BadRequest badRequest);
        T Visit(CreateSessionResult.Forbidden forbidden);
    }
}