using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord
{
    public abstract class GetDetailedTestResult
    {
        public abstract T Accept<T>(IDetailedTestResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : GetDetailedTestResult
        {
            public TestResultResponse Response { get; }

            public SuccessfullyRetrieved(TestResultResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IDetailedTestResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Unsuccessful : GetDetailedTestResult
        {
            public override T Accept<T>(IDetailedTestResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class SupplierBadData : GetDetailedTestResult
        {
            public override T Accept<T>(IDetailedTestResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
