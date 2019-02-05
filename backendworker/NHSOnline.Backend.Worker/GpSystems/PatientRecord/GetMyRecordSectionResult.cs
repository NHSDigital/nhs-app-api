using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord
{
    public abstract class GetMyRecordSectionResult
    {
        public abstract T Accept<T>(IMyRecordSectionResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : GetMyRecordSectionResult
        {
            public MyRecordSectionResponse Response { get; }

            public SuccessfullyRetrieved(MyRecordSectionResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IMyRecordSectionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Unsuccessful : GetMyRecordSectionResult
        {
            public override T Accept<T>(IMyRecordSectionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class SupplierBadData : GetMyRecordSectionResult
        {
            public override T Accept<T>(IMyRecordSectionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidRequest : GetMyRecordSectionResult
        {
            public override T Accept<T>(IMyRecordSectionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class ErrorProcessingSecurityHeader : GetMyRecordSectionResult
        {
            public override T Accept<T>(IMyRecordSectionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidUserCredentials : GetMyRecordSectionResult
        {
            public override T Accept<T>(IMyRecordSectionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class UnknownError : GetMyRecordSectionResult
        {
            public override T Accept<T>(IMyRecordSectionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : GetMyRecordSectionResult
        {
            public override T Accept<T>(IMyRecordSectionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}