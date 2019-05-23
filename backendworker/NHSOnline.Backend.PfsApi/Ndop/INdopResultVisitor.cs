namespace NHSOnline.Backend.PfsApi.Ndop
{
    public interface INdopResultVisitor<out T>
    {
        T Visit(GetNdopResult.Success result);

        T Visit(GetNdopResult.InternalServerError result);
    }
}