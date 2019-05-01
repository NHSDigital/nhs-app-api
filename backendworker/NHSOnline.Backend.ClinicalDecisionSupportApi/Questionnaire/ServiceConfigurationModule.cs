using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire
{
    public class ServiceConfigurationModule : NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IHtmlSanitizer, HtmlSanitizer>();
            
            services.AddTransient<IQuestionnaireService, QuestionnaireService>();
            services.AddTransient<QuestionnaireService>();          
            
            services.AddTransient<IQuestionnaireClient, QuestionnaireClient>();
            services.AddHttpClient<QuestionnaireHttpClient>();
            
            services.AddTransient<IQuestionnaireConfig, QuestionnaireConfig>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}