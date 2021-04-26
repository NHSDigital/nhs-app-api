using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.Staging
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class StagingConfiguration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new StagingLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new ExtNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new StagingNhsAppWebConfiguration();
        public INhsAppApiConfiguration NhsAppApi { get; } = new StagingNhsAppApiConfiguration();
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new StagingNhsExternalServicesConfiguration();
    }
}
