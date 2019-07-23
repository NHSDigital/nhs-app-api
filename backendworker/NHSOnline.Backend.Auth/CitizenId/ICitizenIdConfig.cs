using System;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface ICitizenIdConfig
    {
        Uri CitizenIdApiBaseUrl { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string Issuer { get; set; }
    }
}