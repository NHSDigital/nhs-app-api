using System;

namespace NHSOnline.HttpMocks.Domain
{
    public abstract class Patient
    {
        private static readonly NhsNumberGenerator NhsNumberGenerator = new NhsNumberGenerator();

        public string Id { get; internal set; } = Guid.NewGuid().ToString();

        public NhsNumber NhsNumber { get; } = NhsNumberGenerator.Next();

        public PatientPersonalDetails PersonalDetails { get; } = new PatientPersonalDetails();

        public abstract string VectorOfTrust { get; }
        public abstract string ProofingLevel { get; }
    }
}