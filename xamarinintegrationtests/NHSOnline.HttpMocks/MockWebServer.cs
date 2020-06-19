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
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Vision;

namespace NHSOnline.HttpMocks
{
    public sealed class MockWebServer: IAsyncDisposable
    {
        private readonly IHost _host;
        private readonly Task _task;

        private MockWebServer(IHost host, Task task)
        {
            _host = host;
            _task = task;
        }

        public static MockWebServer Start(IPatients patients)
        {
            var host = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(config => config.AddJsonFile("appsettings.json").AddEnvironmentVariables())
                .ConfigureLogging(config => config.AddConsole())
                .ConfigureWebHost(builder => builder
                    .UseKestrel()
                    .UseUrls("http://*:8080")
                    .Configure(app => app
                        .UseStaticFiles()
                        .UseRouting()
                        .UseEndpoints(endpoints => endpoints.MapControllers()))
                    .ConfigureServices(services => services
                        .PostConfigure<HostFilteringOptions>(options => options.AllowedHosts = new[] {"*"})
                        .AddRouting()
                        .AddControllers()
                        .AddXmlSerializerFormatters())
                    .ConfigureServices(services => services
                        .AddSingleton(patients)
                        .RegisterVisionService()))
                .Build();

            var task = Task.Run(async () => await host.StartAsync());
            
            return new MockWebServer(host, task);
        }

        public async ValueTask DisposeAsync()
        {
            await _host.StopAsync();
            _host.Dispose();
            await _task;
        }
    }
}
