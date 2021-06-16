using NHSOnline.App.Api.Configuration;

namespace NHSOnline.App.DependencyServices
{
    public interface INativeAppVersionCheckService
    {
        bool MeetsMinimumVersion(VersionConfiguration versionConfiguration);
    }
}