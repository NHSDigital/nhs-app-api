namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal interface ITagAssertion
    {
        void Write(ITagLengthValueWriter writer);
    }
}