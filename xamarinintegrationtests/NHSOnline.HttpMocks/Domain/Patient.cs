using System;

namespace NHSOnline.HttpMocks.Domain
{
    public abstract class Patient
    {
        private static readonly NhsNumberGenerator NhsNumberGenerator = new NhsNumberGenerator();
        private static readonly NhsLoginIdGenerator NhsLoginIdGenerator = new NhsLoginIdGenerator();

        protected Patient()
        {
            Login = Id;
        }

        public string Id { get; } = NhsLoginIdGenerator.Next();

        public string Login { get; internal set; }

        public NhsNumber NhsNumber { get; internal set; } = NhsNumberGenerator.Next();

        public PatientPersonalDetails PersonalDetails { get; } = new PatientPersonalDetails();

        public string Scope { get; internal set; } = string.Empty;

        internal Behaviours Behaviours { get; } = new Behaviours();

        public abstract string VectorOfTrust { get; internal set; }
        public abstract string ProofingLevel { get; internal set; }
    }
}
