namespace NHSOnline.Backend.UserInfo.UnitTests.Areas.UserInfo
{
    public class TestUserInfoConfiguration : IUserInfoConfiguration
    {
        public bool SaveToSecondaryContainers { get; set; }
        public bool ReadFromSecondaryContainers { get; set; }
    }
}