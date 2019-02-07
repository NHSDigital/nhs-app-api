using NHSOnline.Backend.Worker.Areas.OrganDonation;
using NHSOnline.Backend.Worker.GpSystems.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public abstract class OrganDonationRegistrationResult
    {
        public abstract T Accept<T>(IOrganDonationRegistrationResultVisitor<T> visitor);
        
        public class SuccessfullyRegistered : OrganDonationRegistrationResult
        {
            public OrganDonationRegistrationResponse Response { get; }
            
            public SuccessfullyRegistered(OrganDonationRegistrationResponse response)
            {
                Response = response;
            }
            
            public override T Accept<T>(IOrganDonationRegistrationResultVisitor<T> visitor) => visitor.Visit(this);
        }
        
        public class SystemError : OrganDonationRegistrationResult
        {
            public override T Accept<T>(IOrganDonationRegistrationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class UpstreamError : OrganDonationRegistrationResult
        {
            public override T Accept<T>(IOrganDonationRegistrationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class Timeout : OrganDonationRegistrationResult
        {
            public override T Accept<T>(IOrganDonationRegistrationResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}