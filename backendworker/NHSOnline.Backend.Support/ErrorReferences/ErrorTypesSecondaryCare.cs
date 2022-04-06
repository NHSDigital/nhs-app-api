using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Support
{
    public abstract partial class ErrorTypes
    {
        public class SecondaryCareTimeout : ErrorTypes
        {
            public override string Prefix => "zu";
            public override ErrorCategory Category => ErrorCategory.SecondaryCare;
            public override int StatusCode => StatusCodes.Status504GatewayTimeout;
        }

        public class SecondaryCareBadResponse : ErrorTypes
        {
            public override string Prefix => "4u";
            public override ErrorCategory Category => ErrorCategory.SecondaryCare;
            public override int StatusCode => StatusCodes.Status502BadGateway;
        }
    }
}