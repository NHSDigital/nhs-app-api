using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.OnboardingAos
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class OnboardingAosConfiguration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new OnboardingAosLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new AosNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new OnboardingAosNhsAppWebConfiguration();
        public INhsAppApiConfiguration NhsAppApi { get; } = new OnboardingAosNhsAppApiConfiguration();
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new OnboardingAosNhsExternalServicesConfiguration();
    }
}
