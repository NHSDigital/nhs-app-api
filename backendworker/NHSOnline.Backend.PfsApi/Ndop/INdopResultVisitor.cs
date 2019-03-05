namespace NHSOnline.Backend.PfsApi.Ndop
{
    public interface INdopResultVisitor<out T>
    {
        T Visit(GetNdopResult.SuccessfullyRetrieved result);

        T Visit(GetNdopResult.Unsuccessful result);
    }
}