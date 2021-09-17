using System.Diagnostics.CodeAnalysis;
using NHSOnline.App.Config.Values.NhsLogin;

namespace NHSOnline.App.Config.Values.Production
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class ProductionConfiguration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new ProductionLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new ProductionNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new ProductionNhsAppWebConfiguration();
        public INhsAppApiConfiguration NhsAppApi { get; } = new ProductionNhsAppApiConfiguration();
        public INhsExternalServicesConfiguration NhsExternalServices { get; } = new ProductionNhsExternalServicesConfiguration();
    }
}
