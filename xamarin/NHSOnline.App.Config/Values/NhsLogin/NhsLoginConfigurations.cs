namespace NHSOnline.App.Config.Values.NhsLogin
{
    internal static class NhsLoginConfigurations
    {
#if NHSLOGIN_ENV_PRODUCTION
        internal static INhsLoginConfiguration NhsLoginConfiguration { get; } = new ProductionNhsLoginConfiguration();
#elif NHSLOGIN_ENV_EXT
        internal static INhsLoginConfiguration NhsLoginConfiguration { get; } = new ExtNhsLoginConfiguration();
#elif NHSLOGIN_ENV_AOS
        internal static INhsLoginConfiguration NhsLoginConfiguration { get; } = new AosNhsLoginConfiguration();
#elif NHSLOGIN_ENV_SANDPIT
        internal static INhsLoginConfiguration NhsLoginConfiguration { get; } = new SandpitNhsLoginConfiguration();
#elif NHSLOGIN_ENV_STUBBED
        internal static INhsLoginConfiguration NhsLoginConfiguration { get; } = new StubbedNhsLoginConfiguration();
#elif NHSLOGIN_ENV_DEV10
        internal static INhsLoginConfiguration NhsLoginConfiguration { get; } = new Dev10NhsLoginConfiguration();
#elif NHSLOGIN_ENV_DEV18
        internal static INhsLoginConfiguration NhsLoginConfiguration { get; } = new Dev18NhsLoginConfiguration();
#else
        internal static INhsLoginConfiguration NhsLoginConfiguration { get; } = new ProductionNhsLoginConfiguration();
#endif
    }
}