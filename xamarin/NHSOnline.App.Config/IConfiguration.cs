namespace NHSOnline.App.Config
{
    public interface IConfiguration
    {
#if ENV_LOCAL
        static IConfiguration Configuration { get; } = new Values.Local.LocalConfiguration();
#elif ENV_STAGING
        static IConfiguration Configuration { get; } = new Values.Staging.StagingConfiguration();
#elif ENV_PREVIEW
        static IConfiguration Configuration { get; } = new Values.Preview.PreviewConfiguration();
#else
#error No supported environemnt defined
#endif

        ILoggingConfiguration Logging { get; }

        INhsLoginConfiguration NhsLogin { get; }
        INhsAppWebConfiguration NhsAppWeb { get; }
        INhsAppApiConfiguration NhsAppApi { get; }
        INhsExternalServicesConfiguration NhsExternalServices { get; }
    }
}
