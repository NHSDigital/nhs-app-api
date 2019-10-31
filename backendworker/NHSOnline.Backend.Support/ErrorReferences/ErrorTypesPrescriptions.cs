using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Support
{
    public abstract partial class ErrorTypes
    {
        public class PrescriptionsBadRequest : ErrorTypes
        {
            public override string Prefix => "5a";
            public override ErrorCategory Category => ErrorCategory.Prescriptions;
            public override int StatusCode => StatusCodes.Status400BadRequest;
        }

        public class PrescriptionsForbidden : ErrorTypes
        {
            public override string Prefix => "5c";
            public override ErrorCategory Category => ErrorCategory.Prescriptions;
            public override int StatusCode => StatusCodes.Status403Forbidden;
        }

        public class PrescriptionsConflict : ErrorTypes
        {
            public override string Prefix => "5f";
            public override ErrorCategory Category => ErrorCategory.Prescriptions;
            public override int StatusCode => StatusCodes.Status409Conflict;
        }

        public class PrescriptionsMedicationAlreadyOrderedWithinLast30Days : ErrorTypes
        {
            public override string Prefix => "5g";
            public override ErrorCategory Category => ErrorCategory.Prescriptions;
            public override int StatusCode => Constants.CustomHttpStatusCodes.Status466MedicationAlreadyOrderedWithinLast30Days;
        }

        public class PrescriptionsUnexpectedError : ErrorTypes
        {
            public override string Prefix => "5k";
            public override ErrorCategory Category => ErrorCategory.Prescriptions;
            public override int StatusCode => StatusCodes.Status500InternalServerError;
        }

        public class PrescriptionsBadGatewayEmis : ErrorTypes
        {
            public override string Prefix => "5e";
            public override ErrorCategory Category => ErrorCategory.Prescriptions;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Emis;
        }

        public class PrescriptionsBadGatewayMicrotest : ErrorTypes
        {
            public override string Prefix => "5m";
            public override ErrorCategory Category => ErrorCategory.Prescriptions;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Microtest;
        }

        public class PrescriptionsBadGatewayTpp : ErrorTypes
        {
            public override string Prefix => "5t";
            public override ErrorCategory Category => ErrorCategory.Prescriptions;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Tpp;
        }

        public class PrescriptionsBadGatewayVision : ErrorTypes
        {
            public override string Prefix => "5s";
            public override ErrorCategory Category => ErrorCategory.Prescriptions;
            public override int StatusCode => StatusCodes.Status502BadGateway;
            public override SourceApi SourceApi => SourceApi.Vision;
        }
    }
}