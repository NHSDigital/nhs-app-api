using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.App.Config.Values.Staging
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Enabled by build configuration")]
    internal sealed class StagingConfiguration : IConfiguration
    {
        public ILoggingConfiguration Logging { get; } = new StagingLoggingConfiguration();
        public INhsLoginConfiguration NhsLogin { get; } = new StagingNhsLoginConfiguration();
        public INhsAppWebConfiguration NhsAppWeb { get; } = new StagingNhsAppWebConfiguration();
        public INhsAppApiConfiguration NhsAppApi { get; } = new StagingNhsAppApiConfiguration();
        public IBeforeYouStartConfiguration BeforeYouStart { get; } = new StagingBeforeYouStartConfiguration();
    }
}
