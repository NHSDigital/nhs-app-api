namespace NHSOnline.Backend.Worker.OrganDonation
{
    public interface IOrganDonationResultVisitor<out T>
    {
        T Visit(OrganDonationResult.NewRegistration result);
        T Visit(OrganDonationResult.ExistingRegistration result);
        T Visit(OrganDonationResult.DemographicsRetrievalFailed result);
        T Visit(OrganDonationResult.DemographicsForbidden result);
        T Visit(OrganDonationResult.DemographicsInternalServerError result);
        T Visit(OrganDonationResult.DemographicsBadGateway result);
        T Visit(OrganDonationResult.SearchSystemUnavailable result);
        T Visit(OrganDonationResult.BadSearchRequest result);
        T Visit(OrganDonationResult.SearchTimeout result);
        T Visit(OrganDonationResult.SearchError result);
    }
}