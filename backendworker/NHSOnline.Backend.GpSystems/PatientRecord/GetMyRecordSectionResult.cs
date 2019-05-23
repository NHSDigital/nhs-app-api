using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public abstract class GetMyRecordSectionResult
    {
        public abstract T Accept<T>(IMyRecordSectionResultVisitor<T> visitor);

        public class Success : GetMyRecordSectionResult
        {
            public MyRecordSectionResponse Response { get; }

            public Success(MyRecordSectionResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IMyRecordSectionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : GetMyRecordSectionResult
        {
            public override T Accept<T>(IMyRecordSectionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : GetMyRecordSectionResult
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