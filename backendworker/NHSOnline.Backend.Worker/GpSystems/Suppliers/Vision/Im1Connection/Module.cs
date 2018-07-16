﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Im1Connection
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<VisionIm1ConnectionService>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}