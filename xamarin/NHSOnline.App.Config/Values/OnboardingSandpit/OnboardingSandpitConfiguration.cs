using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.OnboardingSandpit
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class OnboardingSandpitConfiguration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new OnboardingSandpitLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new SandpitNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new OnboardingSandpitNhsAppWebConfiguration();
        public INhsAppApiConfiguration NhsAppApi { get; } = new OnboardingSandpitNhsAppApiConfiguration();
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new OnboardingSandpitNhsExternalServicesConfiguration();
    }
}
