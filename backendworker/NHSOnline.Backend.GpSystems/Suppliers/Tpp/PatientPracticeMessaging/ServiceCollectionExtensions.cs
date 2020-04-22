using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppPatientPracticeMessagingServices(this IServiceCollection services)
        {
            services.AddTransient<TppPatientMessagesService>();

            services.AddTransient<ITppPatientMessagesMapper, TppPatientMessagesMapper>();
            services.AddTransient<IMessageRecipientsMapper, MessageRecipientsMapper>();
            services
                .AddTransient<IGetPatientPracticeMessagingRecipientsTaskChecker,
                    GetPatientPracticeMessagingRecipientsTaskChecker>();
            services.AddTransient<ITppPatientMessagesUnreadIdsMapper, TppPatientMessagesUnreadIdsMapper > ();

            return services;
        }
    }
}