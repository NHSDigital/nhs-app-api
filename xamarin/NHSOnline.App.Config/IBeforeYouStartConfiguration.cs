using System;

namespace NHSOnline.App.Config
{
    public interface IBeforeYouStartConfiguration
    {
        Uri NhsUkCovidUrl { get; }
        Uri NhsUkConditionsUrl { get; }
        Uri OneOneOneUrl { get; }
    }
}