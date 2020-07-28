namespace NHSOnline.Backend.Support
{
    public interface IGpUserSessionVisitor<T>
    {
        T Visit(NullGpSession nullGpSession);

        T Visit(GpUserSession gpSession);
    }
}