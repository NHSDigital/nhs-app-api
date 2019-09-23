using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public abstract class GetPatientDocumentResult
    {
        public abstract T Accept<T>(IPatientDocumentResultVisitor<T> visitor);

        public class Success : GetPatientDocumentResult
        {
            public PatientDocument Response { get; }

            public Success(PatientDocument response)
            {
                Response = response;
            }

            public override T Accept<T>(IPatientDocumentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }
        
        public class BadGateway : GetPatientDocumentResult
        {
            public override T Accept<T>(IPatientDocumentResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
