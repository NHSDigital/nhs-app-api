namespace NHSOnline.Backend.UserInfo
{
    public interface IUserInfoConfiguration
    {
        bool SaveToSecondaryContainers { get; }
        bool ReadFromSecondaryContainers { get; }
    }
}