using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.App.Config.Values.Preview
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class PreviewConfiguration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new PreviewLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new PreviewNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new PreviewNhsAppWebConfiguration();
        public INhsAppApiConfiguration NhsAppApi { get; } = new PreviewNhsAppApiConfiguration();
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new PreviewNhsExternalServicesConfiguration();
    }
}
