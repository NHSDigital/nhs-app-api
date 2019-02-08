using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public abstract class OrganDonationReferenceDataResult
    {
        public abstract T Accept<T>(IOrganDonationReferenceDataResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : OrganDonationReferenceDataResult
        {

            public OrganDonationReferenceDataResponse Response { get; }
            
            public SuccessfullyRetrieved(OrganDonationReferenceDataResponse response)
            {
                Response = response;
            }
            
            public override T Accept<T>(IOrganDonationReferenceDataResultVisitor<T> visitor) => visitor.Visit(this); 
        }

        public class UpstreamError : OrganDonationReferenceDataResult
        {
            public override T Accept<T>(IOrganDonationReferenceDataResultVisitor<T> visitor) => visitor.Visit(this); 
        }
        
        public class SystemError : OrganDonationReferenceDataResult
        {
            public override T Accept<T>(IOrganDonationReferenceDataResultVisitor<T> visitor) => visitor.Visit(this); 
        }

        public class Timeout : OrganDonationReferenceDataResult
        {
            public override T Accept<T>(IOrganDonationReferenceDataResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}