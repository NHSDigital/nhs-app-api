using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public abstract class GetDetailedTestResult
    {
        public abstract T Accept<T>(IDetailedTestResultVisitor<T> visitor);

        public class Success : GetDetailedTestResult
        {
            public TestResultResponse Response { get; }

            public Success(TestResultResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IDetailedTestResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadGateway : GetDetailedTestResult
        {
            public override T Accept<T>(IDetailedTestResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
