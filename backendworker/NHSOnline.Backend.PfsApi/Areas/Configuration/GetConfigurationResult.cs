using System;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration
{
    public abstract class GetConfigurationResult
    {
        public abstract T Accept<T>(IGetConfigurationResultVisitor<T> visitor);

        public class Success : GetConfigurationResult
        {
            public Success(bool isDeviceSupported, Uri fidoServerUrl)
            {
                Response = new GetConfigurationResponse
                {
                    IsDeviceSupported = isDeviceSupported,
                    FidoServerUrl = fidoServerUrl
                };
            }

            public GetConfigurationResponse Response { get; set; }

            public override T Accept<T>(IGetConfigurationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : GetConfigurationResult
        {
            public override T Accept<T>(IGetConfigurationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : GetConfigurationResult
        {
            public override T Accept<T>(IGetConfigurationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
