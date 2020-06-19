namespace NHSOnline.HttpMocks.Domain
{
    public sealed class PatientPersonalDetails
    {
        public PatientName Name { get; internal set; } = new PatientNameBuilder().Build();
        public PatientAge Age { get; } = new PatientAge("1980-02-03");
        public Gender Gender { get; } = Gender.NotSpecified;
        public PatientContactDetails ContactDetails { get; } = new PatientContactDetails("bob.job@example.com");
    }
}