namespace NHSOnline.Backend.Worker.Suppliers
{
    public interface ISystemProvider
    {
        INhsNumberProvider GetNhsNumberProvider();
    }
}
