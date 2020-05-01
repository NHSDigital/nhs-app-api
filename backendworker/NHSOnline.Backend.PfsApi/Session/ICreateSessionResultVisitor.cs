namespace NHSOnline.Backend.PfsApi.Session
{
    internal interface ICreateSessionResultVisitor<out T>
    {
        T Visit(CreateSessionResult.Success success);
        T Visit(CreateSessionResult.Error error);
    }
}