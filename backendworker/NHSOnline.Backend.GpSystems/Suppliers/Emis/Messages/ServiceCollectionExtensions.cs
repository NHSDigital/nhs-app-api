using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEmisPatientMessagesServices(this IServiceCollection services)
        {
            services.AddTransient<EmisPatientMessagesService>();
            services.AddTransient<IEmisPatientMessagesMapper, EmisPatientMessagesMapper>();
            services.AddTransient<IEmisPatientMessageMapper, EmisPatientMessageMapper>();
            services.AddTransient<IEmisPatientMessageSendMapper, EmisPatientMessageSendMapper>();
            services.AddTransient<IEmisPatientMessageRecipientsMapper, EmisPatientMessageRecipientsMapper>();

            return services;
        }
    }
}