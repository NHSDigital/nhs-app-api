using System;
using NHSOnline.Backend.Worker.Areas.Configuration.Models;

namespace NHSOnline.Backend.Worker.Areas.Configuration
{
    public abstract class GetConfigurationResult
    {
        private GetConfigurationResult()
        {
        }

        public abstract T Accept<T>(IGetConfigurationResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : GetConfigurationResult
        {
            public SuccessfullyRetrieved(bool isDeviceSupported, bool isThrottlingEnabled, Uri fidoServerUrl)
            {
                Response = new GetConfigurationResponse
                {
                    IsDeviceSupported = isDeviceSupported,
                    IsThrottlingEnabled = isThrottlingEnabled,
                    FidoServerUrl = fidoServerUrl
                };
            }

            public GetConfigurationResponse Response { get; set; }

            public override T Accept<T>(IGetConfigurationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class MissingDetailsResult : GetConfigurationResult
        {
            public override T Accept<T>(IGetConfigurationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidNativeAppVersionResult : GetConfigurationResult
        {
            public override T Accept<T>(IGetConfigurationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidDeviceNameResult : GetConfigurationResult
        {
            public override T Accept<T>(IGetConfigurationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class ErrorRetrievingConfigResult : GetConfigurationResult
        {
            public override T Accept<T>(IGetConfigurationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
