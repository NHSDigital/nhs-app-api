using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public abstract class GetHistoricTestResultsResult
    {
        public abstract T Accept<T>(IHistoricTestResultsVisitor<T> visitor);

        public class Success : GetHistoricTestResultsResult
        {
            public TestResults Response { get; }

            public Success(TestResults response)
            {
                Response = response;
            }

            public override T Accept<T>(IHistoricTestResultsVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : GetHistoricTestResultsResult
        {
            public override T Accept<T>(IHistoricTestResultsVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
