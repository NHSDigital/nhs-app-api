namespace NHSOnline.Backend.Messages
{
    public interface IMessagesConfiguration
    {
        bool SenderIdEnabled { get; }
        string SenderIdNhsApp { get; }
    }
}