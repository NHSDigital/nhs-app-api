namespace NHSOnline.HttpMocks.Domain
{
    public sealed class PatientNameBuilder
    {
        private readonly BuilderFieldSetter<PatientNameBuilder> _fieldSetter;

        private string _title = "Mr";
        private string _givenName = "Bob";
        private string _familyName = "Job";

        public PatientNameBuilder() => _fieldSetter = new BuilderFieldSetter<PatientNameBuilder>(this);

        public PatientNameBuilder Title(string title) => _fieldSetter.Set(b => b._title = title);
        public PatientNameBuilder GivenName(string givenName) => _fieldSetter.Set(b => b._givenName = givenName);
        public PatientNameBuilder FamilyName(string familyName) => _fieldSetter.Set(b => b._familyName = familyName);

        internal PatientName Build() => new PatientName(_title, _givenName, _familyName);
    }
}