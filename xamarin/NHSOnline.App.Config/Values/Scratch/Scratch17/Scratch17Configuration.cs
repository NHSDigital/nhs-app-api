using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.Scratch.Scratch17
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class Scratch17Configuration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new ScratchLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new ExtNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new ScratchNhsAppWebConfiguration("scratch17");
        public INhsAppApiConfiguration NhsAppApi { get; } = new ScratchNhsAppApiConfiguration("scratch17");
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new ScratchNhsExternalServicesConfiguration();
    }
}