using System;
using System.Collections.Generic;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration
{
    public abstract class GetConfigurationResultV2
    {
        public abstract T Accept<T>(IGetConfigurationResultVisitorV2<T> visitor);

        public class Success : GetConfigurationResultV2
        {
            public Success(Uri fidoServerUrl, string minimumSupportedAndroidVersion, string minimumSupportediOSVersion,
                List<RootService> knownServices)
            {
                Response = new GetConfigurationResponseV2
                {
                    FidoServerUrl = fidoServerUrl,
                    MinimumSupportedAndroidVersion = minimumSupportedAndroidVersion,
                    MinimumSupportediOSVersion = minimumSupportediOSVersion,
                    KnownServices = knownServices,
                };
            }

            public GetConfigurationResponseV2 Response { get; set; }

            public override T Accept<T>(IGetConfigurationResultVisitorV2<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : GetConfigurationResultV2
        {
            public override T Accept<T>(IGetConfigurationResultVisitorV2<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}