namespace NHSOnline.Backend.Worker.Ndop
{
    public interface INdopResultVisitor<out T>
    {
        T Visit(GetNdopResult.SuccessfullyRetrieved result);

        T Visit(GetNdopResult.Unsuccessful result);
    }
}