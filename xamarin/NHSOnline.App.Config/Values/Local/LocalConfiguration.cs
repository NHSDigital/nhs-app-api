using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.Local
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class LocalConfiguration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new LocalLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new StubbedNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new LocalNhsAppWebConfiguration();
        public INhsAppApiConfiguration NhsAppApi { get; } = new LocalNhsAppApiConfiguration();
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new LocalNhsExternalServicesConfiguration();
    }
}
