using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Tpp;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TppPatientRecordService>();
            
            services.AddTransient<ITppMyRecordMapper, TppMyRecordMapper>();
            services.AddTransient<IGetPatientDcrEventsTaskChecker, GetPatientDcrEventsTaskChecker>();
            services.AddTransient<IGetPatientOverviewTaskChecker, GetPatientOverviewTaskChecker>();
            services.AddTransient<IGetPatientTestResultsTaskChecker, GetPatientTestResultsTaskChecker>();
            services.AddTransient<IGetTppDetailedTestResultChecker, GetTppDetailedTestResultChecker>();
            services.AddTransient<ITppDcrEventsMapper, TppDcrEventsMapper>();
            services.AddTransient<ITppDcrEventItemsMapper, TppDcrEventItemsMapper>();
            services.AddTransient<ITppDetailedTestResultMapper, TppDetailedTestResultMapper>();
            services.AddTransient<ITppTestResultsMapper, TppTestResultsMapper>();
            services.AddTransient<TppPatientOverviewMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}