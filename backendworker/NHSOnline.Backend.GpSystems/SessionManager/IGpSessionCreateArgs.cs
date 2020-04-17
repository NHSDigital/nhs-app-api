namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public interface IGpSessionCreateArgs
    {
        IGpSystem GpSystem { get; }
        string Im1ConnectionToken { get; }
        string OdsCode { get; }
        string NhsNumber { get; }
    }
}