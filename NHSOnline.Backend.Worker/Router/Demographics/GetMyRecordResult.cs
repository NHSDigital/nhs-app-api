namespace NHSOnline.Backend.Worker.Router.Demographics
{
    public abstract class GetMyRecordResult
    {
        private GetMyRecordResult()
        {
        }
        
        public abstract T Accept<T>(IMyRecordResultVisitor<T> visitor);
        
        public class SuccessfullyRetrieved : GetMyRecordResult
        {
            public Areas.MyRecord.Models.DemographicsResponse Response { get; }

            public SuccessfullyRetrieved(Areas.MyRecord.Models.DemographicsResponse response)
            {
                Response = response;
            }
            
            public override T Accept<T>(IMyRecordResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class SupplierSystemUnavailable : GetMyRecordResult
        {
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
    }
}