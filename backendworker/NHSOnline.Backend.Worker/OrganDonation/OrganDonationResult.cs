using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public abstract class OrganDonationResult
    { 

        public abstract T Accept<T>(IOrganDonationResultVisitor<T> visitor);
        
        public class NewRegistration : OrganDonationResult
        {
            public OrganDonationRegistration Registration { get; }

            public NewRegistration(OrganDonationRegistration registration)
            {
                Registration = registration;
            }
        
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }
        
        public class ExistingRegistration : OrganDonationResult
        {
            public OrganDonationRegistration Registration { get; }

            public ExistingRegistration(OrganDonationRegistration registration)
            {
                Registration = registration;
            }
        
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DemographicsBadGateway : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DemographicsRetrievalFailed : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DemographicsForbidden : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DemographicsInternalServerError : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DuplicateRecord : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class SearchSystemUnavailable : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class BadSearchRequest : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class SearchTimeout : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class SearchError : OrganDonationResult
        {
            public override T Accept<T>(IOrganDonationResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}
