using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
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
            base.ConfigureServices(services, configuration);
        }
    }
}