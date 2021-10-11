using NHSOnline.App.Config.Values.Scratch.Scratch12;

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
#elif ENV_SCRATCH1
        static IConfiguration Configuration { get; } = new Values.Scratch1.Scratch1Configuration();
#elif ENV_SCRATCH2
        static IConfiguration Configuration { get; } = new Values.Scratch2.Scratch2Configuration();
#elif ENV_SCRATCH3
        static IConfiguration Configuration { get; } = new Values.Scratch3.Scratch3Configuration();
#elif ENV_SCRATCH4
        static IConfiguration Configuration { get; } = new Values.Scratch4.Scratch4Configuration();
#elif ENV_SCRATCH5
        static IConfiguration Configuration { get; } = new Values.Scratch5.Scratch5Configuration();
#elif ENV_SCRATCH6
        static IConfiguration Configuration { get; } = new Values.Scratch6.Scratch6Configuration();
#elif ENV_SCRATCH7
        static IConfiguration Configuration { get; } = new Values.Scratch7.Scratch7Configuration();
#elif ENV_SCRATCH8
        static IConfiguration Configuration { get; } = new Values.Scratch8.Scratch8Configuration();
#elif ENV_SCRATCH9
        static IConfiguration Configuration { get; } = new Values.Scratch9.Scratch9Configuration();
#elif ENV_SCRATCH10
        static IConfiguration Configuration { get; } = new Values.Scratch10.Scratch10Configuration();
#elif ENV_SCRATCH11
        static IConfiguration Configuration { get; } = new Values.Scratch11.Scratch11Configuration();
#elif ENV_SCRATCH12
        static IConfiguration Configuration { get; } = new Scratch12Configuration();
#elif ENV_SCRATCH13
        static IConfiguration Configuration { get; } = new Values.Scratch13.Scratch13Configuration();
#elif ENV_SCRATCH14
        static IConfiguration Configuration { get; } = new Values.Scratch14.Scratch14Configuration();
#elif ENV_SCRATCH15
        static IConfiguration Configuration { get; } = new Values.Scratch15.Scratch15Configuration();
#elif ENV_SCRATCH16
        static IConfiguration Configuration { get; } = new Values.Scratch16.Scratch16Configuration();
#elif ENV_SCRATCH17
        static IConfiguration Configuration { get; } = new Values.Scratch17.Scratch17Configuration();
#elif ENV_SCRATCH18
        static IConfiguration Configuration { get; } = new Values.Scratch18.Scratch18Configuration();
#elif ENV_SCRATCH19
        static IConfiguration Configuration { get; } = new Values.Scratch19.Scratch19Configuration();
#elif ENV_SCRATCH20
        static IConfiguration Configuration { get; } = new Values.Scratch20.Scratch20Configuration();
#elif ENV_SCRATCH21
        static IConfiguration Configuration { get; } = new Values.Scratch21.Scratch21Configuration();
#elif ENV_PRODUCTION
        static IConfiguration Configuration { get; } = new Values.Production.ProductionConfiguration();
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
