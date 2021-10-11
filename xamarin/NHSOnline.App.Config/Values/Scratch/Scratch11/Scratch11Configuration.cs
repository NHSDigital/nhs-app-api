using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.Scratch.Scratch11
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class Scratch11Configuration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new ScratchLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new ExtNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new ScratchNhsAppWebConfiguration("scratch11");
        public INhsAppApiConfiguration NhsAppApi { get; } = new ScratchNhsAppApiConfiguration("scratch11");
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new ScratchNhsExternalServicesConfiguration();
    }
}