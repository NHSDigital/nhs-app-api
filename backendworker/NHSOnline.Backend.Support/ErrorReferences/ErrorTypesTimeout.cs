using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public abstract partial class ErrorTypes
    {
        public static ErrorTypes LoginTimeout(ILogger logger, Supplier supplier)
        {
            return LookupErrorType(logger, ErrorCategory.Timeout, StatusCodes.Status504GatewayTimeout, SupplierSourceApiConverter.Instance[supplier]);
        }

        public abstract class ErrorTypesTimeout : ErrorTypes
        {
            public override ErrorCategory Category => ErrorCategory.Timeout;

            public override int StatusCode => StatusCodes.Status504GatewayTimeout;
        }

        public class TimeoutEmis : ErrorTypesTimeout
        {
            public override string Prefix => "ze";

            public override SourceApi SourceApi => SourceApi.Emis;
        }

        public class TimeoutFake : ErrorTypesTimeout
        {
            public override string Prefix => "zf";

            public override SourceApi SourceApi => SourceApi.Fake;
        }

        public class TimeoutMicrotest : ErrorTypesTimeout
        {
            public override string Prefix => "zm";

            public override SourceApi SourceApi => SourceApi.Microtest;
        }

        public class TimeoutNhsLogin : ErrorTypesTimeout
        {
            public override string Prefix => "zn";

            public override SourceApi SourceApi => SourceApi.NhsLogin;
        }

        public class TimeoutOrganDonation : ErrorTypesTimeout
        {
            public override string Prefix => "zo";

            public override SourceApi SourceApi => SourceApi.OrganDonation;
        }

        public class TimeoutServiceJourneyRules : ErrorTypesTimeout
        {
            public override string Prefix => "zj";

            public override SourceApi SourceApi => SourceApi.ServiceJourneyRules;
        }

        public class TimeoutTpp : ErrorTypesTimeout
        {
            public override string Prefix => "zt";

            public override SourceApi SourceApi => SourceApi.Tpp;
        }

        public class TimeoutVision : ErrorTypesTimeout
        {
            public override string Prefix => "zs";

            public override SourceApi SourceApi => SourceApi.Vision;
        }
    }
}