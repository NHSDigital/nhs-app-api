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
#elif ENV_ONBOARDING_SANDPIT
        static IConfiguration Configuration { get; } = new Values.OnboardingSandpit.OnboardingSandpitConfiguration();
#elif ENV_ONBOARDING_AOS
        static IConfiguration Configuration { get; } = new Values.OnboardingAos.OnboardingAosConfiguration();
#elif ENV_SCRATCH6
        static IConfiguration Configuration { get; } = new Values.Scratch6.Scratch6Configuration();
#elif ENV_SCRATCH18
        static IConfiguration Configuration { get; } = new Values.Scratch18.Scratch18Configuration();
#elif ENV_SCRATCH19
        static IConfiguration Configuration { get; } = new Values.Scratch19.Scratch19Configuration();
#else
#error No supported environment defined
#endif

        ILoggingConfiguration Logging { get; }

        INhsLoginConfiguration NhsLogin { get; }
        INhsAppWebConfiguration NhsAppWeb { get; }
        INhsAppApiConfiguration NhsAppApi { get; }
        INhsExternalServicesConfiguration NhsExternalServices { get; }
    }
}
