using System;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public interface ISecondaryCareConfig
    {
        Uri BaseUrl { get; }
        string EventsPath { get; }
    }
}