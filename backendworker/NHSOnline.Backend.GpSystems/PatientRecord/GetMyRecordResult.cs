using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public abstract class GetMyRecordResult
    {
        public abstract T Accept<T>(IMyRecordResultVisitor<T> visitor);

        public class Success : GetMyRecordResult
        {
            public MyRecordResponse Response { get; }

            public Success(MyRecordResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IMyRecordResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : GetMyRecordResult
        {
            public override T Accept<T>(IMyRecordResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : GetMyRecordResult
        {
            public override T Accept<T>(IMyRecordResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
