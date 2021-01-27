using System;

namespace NHSOnline.HttpMocks.Domain
{
    public abstract class Patient
    {
        private static readonly NhsNumberGenerator NhsNumberGenerator = new NhsNumberGenerator();

        protected Patient()
        {
            Login = Id;
        }

        public string Id { get; } = Guid.NewGuid().ToString();

        public string Login { get; internal set; }

        public NhsNumber NhsNumber { get; internal set; } = NhsNumberGenerator.Next();

        public PatientPersonalDetails PersonalDetails { get; } = new PatientPersonalDetails();

        internal Behaviours Behaviours { get; } = new Behaviours();

        public abstract string VectorOfTrust { get; }
        public abstract string ProofingLevel { get; }
    }
}
