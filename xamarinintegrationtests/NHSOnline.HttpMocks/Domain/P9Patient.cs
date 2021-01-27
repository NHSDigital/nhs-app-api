namespace NHSOnline.HttpMocks.Domain
{
    public class P9Patient : Patient, IGpRegistered
    {
        public override string VectorOfTrust => "P9.Cp.Cd";
        public override string ProofingLevel => "P9";

        public string OdsCode { get; internal set; } = string.Empty;
        public string Im1ConnectionToken => string.Empty;
    }
}