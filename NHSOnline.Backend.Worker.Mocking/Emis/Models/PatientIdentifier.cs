namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class PatientIdentifier
    {
        public string IdentifierValue { get; }
        public IdentifierType IdentifierType { get; }

        public PatientIdentifier(IdentifierType identifierType, string identifierValue)
        {
            IdentifierType = identifierType;
            IdentifierValue = identifierValue;
        }


        public static PatientIdentifier NHSNumber(string nhsNumber)
        {
            return new PatientIdentifier(IdentifierType.NhsNumber, nhsNumber);
        }
    }
}