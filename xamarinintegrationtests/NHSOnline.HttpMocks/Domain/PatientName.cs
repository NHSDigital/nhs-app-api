namespace NHSOnline.HttpMocks.Domain
{
    public sealed class PatientName
    {
        internal PatientName(string title, string givenName, string familyName)
        {
            Title = title;
            GivenName = givenName;
            FamilyName = familyName;
        }

        public string Title { get; }
        public string GivenName { get; }
        public string FamilyName { get; }
    }
}