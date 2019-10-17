using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts
{
    public abstract class GetPatientGuidResult
    {
        public abstract T Accept<T>(IPatientGuidResultVisitor<T> visitor);

        public class Success : GetPatientGuidResult
        {
            public PatientIdResponse Response { get; }

            public Success(PatientIdResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IPatientGuidResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }        
    }
}