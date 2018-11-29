using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Tpp;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TppCourseService>();
            services.AddTransient<TppPrescriptionService>();
            services.AddTransient<TppPrescriptionRequestValidationService>();

            services.AddTransient<ITppCourseMapper, TppCourseMapper>();
            services.AddTransient<ITppPrescriptionMapper, TppPrescriptionMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
