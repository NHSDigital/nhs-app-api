using System;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface ICitizenIdConfig
    {
        Uri CitizenIdApiBaseUrl { get; set; }
        string ClientId { get; set; }
        string Issuer { get; set; }
        string NhsLoginKeyPath { get; set; }
        string NhsLoginKeyPassword { get; set; }
        string TokenPath { get; set; }
    }
}