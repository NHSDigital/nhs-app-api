using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.Scratch6
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class Scratch6Configuration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new Scratch6LoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new ExtNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new Scratch6NhsAppWebConfiguration();
        public INhsAppApiConfiguration NhsAppApi { get; } = new Scratch6NhsAppApiConfiguration();
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new Scratch6NhsExternalServicesConfiguration();
    }
}
