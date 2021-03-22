using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.App.NhsLogin.Fido.Models
{
    internal enum UafOperation
    {
        Reg,
        Auth,
        [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Required to match UAF specification")]
        Dereg
    }
}