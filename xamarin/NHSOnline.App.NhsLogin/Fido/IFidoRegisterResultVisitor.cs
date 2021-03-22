namespace NHSOnline.App.NhsLogin.Fido
{
    public interface IFidoRegisterResultVisitor<T>
    {
        T Visit(FidoRegisterResult.Registered registered);
        T Visit(FidoRegisterResult.Failed failed);
    }
}