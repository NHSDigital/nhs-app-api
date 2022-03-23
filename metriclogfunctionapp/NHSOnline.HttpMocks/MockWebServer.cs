using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSOnline.HttpMocks.Apple;
using NHSOnline.HttpMocks.Download;
using NHSOnline.HttpMocks.Spine;

namespace NHSOnline.HttpMocks
{
    public sealed class MockWebServer : IAsyncDisposable
    {
        private readonly IHost _host;
        private readonly Task _task;

        private MockWebServer(IHost host, Task task)
        {
            _host = host;
            _task = task;
        }

        public static MockWebServer Start(Action<ILoggingBuilder> configureLogging)
        {
            var host = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureLogging(configureLogging)
                .ConfigureWebHost(ConfigureWebHost)
                .Build();

            var task = Task.Run(async () => await host.StartAsync());

            return new MockWebServer(host, task);
        }

        private static void ConfigureAppConfiguration(IConfigurationBuilder config)
        {
            config
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

        private static void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseKestrel()
                .UseUrls("http://*:8080")
                .Configure(app => app
                    .UseStaticFiles()
                    .UseRouting()
                    .UseEndpoints(endpoints => endpoints.MapControllers()))
                .ConfigureServices(services => services
                    .PostConfigure<HostFilteringOptions>(options => options.AllowedHosts = new[] { "*" })
                    .AddRouting()
                    .AddControllers())
                .ConfigureServices(ConfigureMockServices);
        }

        private static void ConfigureMockServices(IServiceCollection services)
        {
            services.AddSingleton<AppleSalesReportResponse>();
            services.AddSingleton<SpineOrganisationsResponse>();
            services.AddSingleton<DownloadResponse>();
        }

        public SpineOrganisationsResponse SpineOrganisationsResponse => _host.Services.GetRequiredService<SpineOrganisationsResponse>();
        public AppleSalesReportResponse AppleSalesReportResponse => _host.Services.GetRequiredService<AppleSalesReportResponse>();
        public DownloadResponse DownloadResponse => _host.Services.GetRequiredService<DownloadResponse>();

        public async ValueTask DisposeAsync()
        {
            await _host.StopAsync();
            _host.Dispose();
            await _task;
        }
    }
}
