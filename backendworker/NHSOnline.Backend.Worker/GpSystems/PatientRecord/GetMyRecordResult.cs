using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord
{
    public abstract class GetMyRecordResult
    {
        public abstract T Accept<T>(IMyRecordResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : GetMyRecordResult
        {
            public MyRecordResponse Response { get; }

            public SuccessfullyRetrieved(MyRecordResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IMyRecordResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Unsuccessful : GetMyRecordResult
        {
            public override T Accept<T>(IMyRecordResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class SupplierBadData : GetMyRecordResult
        {
            public override T Accept<T>(IMyRecordResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidRequest : GetMyRecordResult
        {
            public override T Accept<T>(IMyRecordResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class ErrorProcessingSecurityHeader : GetMyRecordResult
        {
            public override T Accept<T>(IMyRecordResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InvalidUserCredentials : GetMyRecordResult
        {
            public override T Accept<T>(IMyRecordResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class UnknownError : GetMyRecordResult
        {
            public override T Accept<T>(IMyRecordResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
