namespace NHSOnline.App.NhsLogin.Fido.Assertion
{
    internal interface ITagLengthValueWriter
    {
        TagLengthValueValueWriter StartTag(ushort tag);
    }
}