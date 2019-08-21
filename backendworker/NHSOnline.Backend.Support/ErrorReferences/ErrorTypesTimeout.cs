using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Support
{
    public abstract partial class ErrorTypes
    {
        public class TimeoutEmis : ErrorTypes
        {
            public override string Prefix => "ze";

            public override ErrorCategory Category => ErrorCategory.Timeout;

            public override int StatusCode => StatusCodes.Status504GatewayTimeout;

            public override SourceApi SourceApi => SourceApi.Emis;
        }

        public class TimeoutMicrotest : ErrorTypes
        {
            public override string Prefix => "zm";

            public override ErrorCategory Category => ErrorCategory.Timeout;

            public override int StatusCode => StatusCodes.Status504GatewayTimeout;

            public override SourceApi SourceApi => SourceApi.Microtest;
        }

        public class TimeoutNhsLogin : ErrorTypes
        {
            public override string Prefix => "zn";

            public override ErrorCategory Category => ErrorCategory.Timeout;

            public override int StatusCode => StatusCodes.Status504GatewayTimeout;

            public override SourceApi SourceApi => SourceApi.NhsLogin;
        }

        public class TimeoutOrganDonation : ErrorTypes
        {
            public override string Prefix => "zo";

            public override ErrorCategory Category => ErrorCategory.Timeout;

            public override int StatusCode => StatusCodes.Status504GatewayTimeout;

            public override SourceApi SourceApi => SourceApi.OrganDonation;
        }

        public class TimeoutServiceJourneyRules : ErrorTypes
        {
            public override string Prefix => "zj";

            public override ErrorCategory Category => ErrorCategory.Timeout;

            public override int StatusCode => StatusCodes.Status504GatewayTimeout;

            public override SourceApi SourceApi => SourceApi.ServiceJourneyRules;
        }

        public class TimeoutTpp : ErrorTypes
        {
            public override string Prefix => "zt";

            public override ErrorCategory Category => ErrorCategory.Timeout;

            public override int StatusCode => StatusCodes.Status504GatewayTimeout;

            public override SourceApi SourceApi => SourceApi.Tpp;
        }

        public class TimeoutVision : ErrorTypes
        {
            public override string Prefix => "zs";

            public override ErrorCategory Category => ErrorCategory.Timeout;

            public override int StatusCode => StatusCodes.Status504GatewayTimeout;

            public override SourceApi SourceApi => SourceApi.Vision;
        }
    }
}