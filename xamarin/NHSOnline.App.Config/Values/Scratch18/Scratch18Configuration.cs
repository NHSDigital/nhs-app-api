using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.Scratch18
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class Scratch18Configuration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new Scratch18LoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new ExtNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new Scratch18NhsAppWebConfiguration();
        public INhsAppApiConfiguration NhsAppApi { get; } = new Scratch18NhsAppApiConfiguration();
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new Scratch18NhsExternalServicesConfiguration();
    }
}
