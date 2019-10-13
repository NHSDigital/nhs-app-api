using System;

namespace NHSOnline.Backend.PfsApi.Configs
{
    public interface IApiConfig
    {
        Uri ApiBaseUrl { get; set; }
    }
}