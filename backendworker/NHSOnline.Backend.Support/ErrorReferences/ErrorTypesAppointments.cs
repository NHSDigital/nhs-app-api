using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Support
{
    public abstract partial class ErrorTypes
    {
        public class AppointmentsBadRequest : ErrorTypes
        {
            public override string Prefix => "4a";
            public override ErrorCategory Category => ErrorCategory.Appointments;
            public override int StatusCode => StatusCodes.Status400BadRequest;
        }

        public class AppointmentsForbidden : ErrorTypes
        {
            public override string Prefix => "4c";
            public override ErrorCategory Category => ErrorCategory.Appointments;
            public override int StatusCode => StatusCodes.Status403Forbidden;
        }

        public class AppointmentsConflict : ErrorTypes
        {
            public override string Prefix => "4f";
            public override ErrorCategory Category => ErrorCategory.Appointments;
            public override int StatusCode => StatusCodes.Status409Conflict;
        }

        public class AppointmentsLimitReached : ErrorTypes
        {
            public override string Prefix => "4g";
            public override ErrorCategory Category => ErrorCategory.Appointments;
            public override int StatusCode => Constants.CustomHttpStatusCodes.Status460LimitReached;
        }

        public class AppointmentsTooLateToCancel : ErrorTypes
        {
            public override string Prefix => "4h";
            public override ErrorCategory Category => ErrorCategory.Appointments;
            public override int StatusCode => Constants.CustomHttpStatusCodes.Status461TooLate; 
        }

        public class AppointmentsUnexpectedError : ErrorTypes
        {
            public override string Prefix => "4k";
            public override ErrorCategory Category => ErrorCategory.Appointments;
            public override int StatusCode => StatusCodes.Status500InternalServerError;
        }

        public class AppointmentsBadGatewayEmis : ErrorTypes
        {
            public override string Prefix => "4e";
            public override ErrorCategory Category => ErrorCategory.Appointments;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Emis;
        }

        public class AppointmentsBadGatewayMicrotest : ErrorTypes
        {
            public override string Prefix => "4m";
            public override ErrorCategory Category => ErrorCategory.Appointments;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Microtest;
        }

        public class AppointmentsBadGatewayTpp : ErrorTypes
        {
            public override string Prefix => "4t";
            public override ErrorCategory Category => ErrorCategory.Appointments;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Tpp;
        }

        public class AppointmentsBadGatewayVision : ErrorTypes
        {
            public override string Prefix => "4s";
            public override ErrorCategory Category => ErrorCategory.Appointments;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Vision;
        }
    }
}