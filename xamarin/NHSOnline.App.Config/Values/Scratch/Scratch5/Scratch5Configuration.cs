using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.Scratch.Scratch5
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class Scratch5Configuration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new ScratchLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new ExtNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new ScratchNhsAppWebConfiguration("scratch5");
        public INhsAppApiConfiguration NhsAppApi { get; } = new ScratchNhsAppApiConfiguration("scratch5");
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new ScratchNhsExternalServicesConfiguration();
    }
}