using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.Scratch.Scratch1
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class Scratch1Configuration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new ScratchLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new ExtNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new ScratchNhsAppWebConfiguration("scratch1");
        public INhsAppApiConfiguration NhsAppApi { get; } = new ScratchNhsAppApiConfiguration("scratch1");
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new ScratchNhsExternalServicesConfiguration();
    }
}