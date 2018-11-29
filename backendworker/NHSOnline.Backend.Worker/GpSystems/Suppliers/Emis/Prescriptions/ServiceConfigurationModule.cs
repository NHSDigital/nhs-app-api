using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Emis;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<EmisCourseService>();
            services.AddTransient<EmisPrescriptionService>();
            services.AddTransient<EmisPrescriptionRequestValidationService>();

            services.AddTransient<IEmisPrescriptionMapper, EmisPrescriptionMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
