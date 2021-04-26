using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.Scratch19
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class Scratch19Configuration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new Scratch19LoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new ExtNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new Scratch19NhsAppWebConfiguration();
        public INhsAppApiConfiguration NhsAppApi { get; } = new Scratch19NhsAppApiConfiguration();
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new Scratch19NhsExternalServicesConfiguration();
    }
}
