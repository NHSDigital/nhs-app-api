using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppPatientRecordServices(this IServiceCollection services)
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

            return services;
        }
    }
}