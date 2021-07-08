namespace NHSOnline.HttpMocks.Domain
{
    public class P9Patient : Patient, IGpRegistered
    {
        public override string VectorOfTrust { get; internal set; } = "P9.Cp.Cd";
        public override string ProofingLevel { get; internal set; } = "P9";

        public string OdsCode { get; internal set; } = string.Empty;
        public string Im1ConnectionToken => string.Empty;
    }
}