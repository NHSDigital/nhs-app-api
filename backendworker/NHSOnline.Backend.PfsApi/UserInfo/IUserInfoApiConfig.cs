using System;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public interface IUserInfoApiConfig
    {
        Uri UserInfoApiBaseUrl { get; set; }
    }
}