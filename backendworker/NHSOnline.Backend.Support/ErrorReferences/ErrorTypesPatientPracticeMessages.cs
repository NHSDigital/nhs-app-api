using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Support
{
    public abstract partial class ErrorTypes
    {
        public class PatientPracticeMessagesBadRequest : ErrorTypes
        {
            public override string Prefix => "9a";
            public override ErrorCategory Category => ErrorCategory.PatientPracticeMessages;
            public override int StatusCode => StatusCodes.Status400BadRequest;
        }

        public class PatientPracticeMessagesForbidden : ErrorTypes
        {
            public override string Prefix => "9c";
            public override ErrorCategory Category => ErrorCategory.PatientPracticeMessages;
            public override int StatusCode => StatusCodes.Status403Forbidden;
        }

        public class PatientPracticeMessagesUnexpectedError : ErrorTypes
        {
            public override string Prefix => "9k";
            public override ErrorCategory Category => ErrorCategory.PatientPracticeMessages;
            public override int StatusCode => StatusCodes.Status500InternalServerError;
        }

        public class PatientPracticeMessagesBadGatewayEmis : ErrorTypes
        {
            public override string Prefix => "9e";
            public override ErrorCategory Category => ErrorCategory.PatientPracticeMessages;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Emis;
        }

        public class PatientPracticeMessagesBadGatewayMicrotest : ErrorTypes
        {
            public override string Prefix => "9m";
            public override ErrorCategory Category => ErrorCategory.PatientPracticeMessages;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Microtest;
        }

        public class PatientPracticeMessagesBadGatewayTpp : ErrorTypes
        {
            public override string Prefix => "9t";
            public override ErrorCategory Category => ErrorCategory.PatientPracticeMessages;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Tpp;
        }

        public class PatientPracticeMessagesBadGatewayVision : ErrorTypes
        {
            public override string Prefix => "9s";
            public override ErrorCategory Category => ErrorCategory.PatientPracticeMessages;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Vision;
        }
    }
}