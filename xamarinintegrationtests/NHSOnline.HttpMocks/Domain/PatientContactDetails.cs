namespace NHSOnline.HttpMocks.Domain
{
    public sealed class PatientContactDetails
    {
        public PatientContactDetails(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public string EmailAddress { get; }
    }
}