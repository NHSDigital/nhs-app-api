namespace NHSOnline.Backend.Worker.OrganDonation
{
    public interface IOrganDonationReferenceDataResultVisitor<out T>
    {
        T Visit(OrganDonationReferenceDataResult.SuccessfullyRetrieved result);
        T Visit(OrganDonationReferenceDataResult.SystemError result);
        T Visit(OrganDonationReferenceDataResult.UpstreamError result);
        T Visit(OrganDonationReferenceDataResult.Timeout result);
    }
}