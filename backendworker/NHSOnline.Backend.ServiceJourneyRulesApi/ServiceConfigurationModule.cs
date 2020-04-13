using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps;
using NHSOnline.Backend.ServiceJourneyRulesApi.Service;
using NHSOnline.Backend.Support.Sanitization;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    public class ServiceConfigurationModule: NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var camelCaseNamingConvention = new CamelCaseNamingConvention();

            services.AddTransient<IServiceJourneyRulesService, ServiceJourneyRulesService>();
            services.AddTransient<IProcessState, ProcessState>();
            
            services.AddSingleton<ITextReaderBuilder<TextReader>, StreamReaderBuilder>();
            services.AddSingleton<IParserBuilder<Parser>, ParserBuilder>();
            services.AddSingleton(s => new YamlNodeDeserializerBuilder(s)
                .WithTag(Constants.CustomYamlNodeTags.Include)
                .WithSupportedTypes(new List<Type>{ typeof(PublicHealthNotification) })
                .WithNamingConvention(camelCaseNamingConvention)
                .Build());
            
            services.AddSingleton(new SerializerBuilder()
                .WithNamingConvention(camelCaseNamingConvention)
                .JsonCompatible()
                .Build());
            services.AddSingleton(Assembly.GetExecutingAssembly());

            services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>();
            services.AddSingleton<IYamlReaderFactory, YamlReaderFactory>();
            services.AddSingleton<IYamlWriter, YamlWriter>();
            services.AddSingleton<IYamlToJsonConverter, YamlToJsonConverter>();
            services.AddSingleton<IConfigurationRuleFileValidator, ConfigurationRuleFileValidator>();
            services.AddSingleton<ISchemaValidator, SchemaValidator>();
            services.AddSingleton<IFileHandler, FileHandler>();
            services.AddSingleton<IGpInfoReader, GpInfoReader>();
            services.AddSingleton<IValidatorStep, LoadRequiredFiles>();
            services.AddSingleton<IValidatorStep, LoadConfigurationFiles>();
            services.AddSingleton<IValidatorStep, ValidateUniqueOdsConfiguration>();
            services.AddSingleton<IValidatorStep, MergeOdsJourneys>();
            services.AddSingleton<IValidatorStep, ValidateOdsJourneys>();
            services.AddSingleton<IValidatorStep, OutputOdsJourneys>();
            services.AddSingleton<ILoadStep, LoadRequiredFiles>();
            services.AddSingleton<ILoadStep, LoadConfigurationFiles>();
            services.AddSingleton<ILoadStep, SanitizeOdsJourneys>();
            services.AddSingleton<ILoadStep, ValidateOdsJourneys>();
            services.AddSingleton(typeof(EnumDescriptionConverter<>));
            services.AddSingleton<IServiceJourneyRulesConfiguration, ServiceJourneyRulesConfiguration>();
            services.AddSingleton<IYamlSerializer, YamlSerializer>();
            services.AddSingleton<IDirectory, DirectoryWrapper>();
            services.AddSingleton<IJourneyService, JourneyService>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}