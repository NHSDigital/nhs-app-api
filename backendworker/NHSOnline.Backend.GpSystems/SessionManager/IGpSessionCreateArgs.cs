namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public interface IGpSessionCreateArgs
    {
        string Im1ConnectionToken { get; }
        string OdsCode { get; }
        string NhsNumber { get; }
    }
}