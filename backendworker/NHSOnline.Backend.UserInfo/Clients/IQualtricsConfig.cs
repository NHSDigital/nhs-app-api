using System;

namespace NHSOnline.Backend.UserInfo.Clients
{
    public interface IQualtricsConfig
    {
        Uri BaseUrl { get; set; }
        string DirectoryId{ get; set; }
        string MailingListId{ get; set; }
        string Token { get; set; }
    }
}