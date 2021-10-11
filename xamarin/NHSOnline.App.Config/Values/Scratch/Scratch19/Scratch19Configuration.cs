using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.Scratch.Scratch19
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class Scratch19Configuration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new ScratchLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new ExtNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new ScratchNhsAppWebConfiguration("scratch19");
        public INhsAppApiConfiguration NhsAppApi { get; } = new ScratchNhsAppApiConfiguration("scratch19");
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new ScratchNhsExternalServicesConfiguration();
    }
}
