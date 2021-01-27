namespace NHSOnline.HttpMocks.Domain
{
    public class P5Patient: Patient
    {
        public override string VectorOfTrust => "P5.Cp.Cd";
        public override string ProofingLevel => "P5";
    }
}