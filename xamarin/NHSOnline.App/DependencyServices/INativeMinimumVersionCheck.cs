using NHSOnline.App.Api.Configuration;

namespace NHSOnline.App.DependencyServices
{
    public interface INativeMinimumVersionCheck
    {
        bool MeetsMinimumVersion(VersionConfiguration versionConfiguration);
    }
}