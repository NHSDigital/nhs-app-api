using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public abstract partial class ErrorTypes
    {
        public static ErrorTypes LoginBadGateway(ILogger logger, Supplier supplier)
        {
            return LookupErrorType(logger, ErrorCategory.Login, StatusCodes.Status502BadGateway, SupplierSourceApiConverter.Instance[supplier]);
        }

        public abstract class ErrorTypeBadGateway: ErrorTypes
        {
            public override ErrorCategory Category => ErrorCategory.Login;

            public override int StatusCode => StatusCodes.Status502BadGateway;
        }

        public class LoginBadRequest : ErrorTypes
        {
            public override string Prefix => "3a";

            public override ErrorCategory Category => ErrorCategory.Login;

            public override int StatusCode => StatusCodes.Status400BadRequest;
        }

        public class LoginForbidden : ErrorTypes
        {
            public override string Prefix => "3c";

            public override ErrorCategory Category => ErrorCategory.Login;

            public override int StatusCode => StatusCodes.Status403Forbidden;
        }

        public class LoginOdsCodeNotFoundOrNotSupported : ErrorTypes
        {
            public override string Prefix => "3f";

            public override ErrorCategory Category => ErrorCategory.Login;

            public override int StatusCode => Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber;
        }

        public class GPSessionUnavailable: ErrorTypes
        {
            private readonly ErrorTypes _cause;

            public GPSessionUnavailable()
            {
            }

            public GPSessionUnavailable(ErrorTypes cause)
            {
                _cause = cause;
            }

            public override string Prefix => _cause?.Prefix ?? "3u";

            public override ErrorCategory Category => _cause?.Category ?? ErrorCategory.Login;

            public override int StatusCode => Constants.CustomHttpStatusCodes.Status599GpSessionUnavailable;
        }

        public class LoginMinimumAgeNotMet : ErrorTypes
        {
            public override string Prefix => "3g";

            public override ErrorCategory Category => ErrorCategory.Login;

            public override int StatusCode => Constants.CustomHttpStatusCodes.Status465FailedAgeRequirement;
        }

        public class LoginUnexpectedError : ErrorTypes
        {
            public override string Prefix => "3h";

            public override ErrorCategory Category => ErrorCategory.Login;

            public override int StatusCode => StatusCodes.Status500InternalServerError;
        }

        public class LoginServiceJourneyRulesOtherError : ErrorTypes
        {
            public override string Prefix => "3k";

            public override ErrorCategory Category => ErrorCategory.Login;

            public override int StatusCode => StatusCodes.Status500InternalServerError;

            public override SourceApi SourceApi => SourceApi.ServiceJourneyRules;
        }

        public class LoginBadGatewayEmis : ErrorTypeBadGateway
        {
            public override string Prefix => "3e";

            public override SourceApi SourceApi => SourceApi.Emis;
        }

        public class LoginBadGatewayFakeGp : ErrorTypeBadGateway
        {
            public override string Prefix => "3f";

            public override SourceApi SourceApi => SourceApi.Fake;
        }

        public class LoginGPUnparseable : ErrorTypes
        {
            public override string Prefix => "3p";

            public override ErrorCategory Category => ErrorCategory.Login;

            public override int StatusCode => Constants.CustomHttpStatusCodes.Status599GpSessionUnavailable;
        }

        public class LoginErrorExceptionResult : ErrorTypes
        {
            public override string Prefix => "3ee";

            public override ErrorCategory Category => ErrorCategory.Login;

            public override int StatusCode => Constants.CustomHttpStatusCodes.Status599GpSessionUnavailable;
        }

        public class LoginBadGatewayTpp : ErrorTypeBadGateway
        {
            public override string Prefix => "3t";

            public override SourceApi SourceApi => SourceApi.Tpp;
        }

        public class LoginBadGatewayMicrotest : ErrorTypeBadGateway
        {
            public override string Prefix => "3m";

            public override SourceApi SourceApi => SourceApi.Microtest;
        }

        public class LoginBadGatewayVision : ErrorTypeBadGateway
        {
            public override string Prefix => "3s";

            public override SourceApi SourceApi => SourceApi.Vision;
        }

        public class LoginBadGatewayNhsLogin : ErrorTypeBadGateway
        {
            public override string Prefix => "3n";

            public override SourceApi SourceApi => SourceApi.NhsLogin;
        }
    }
}