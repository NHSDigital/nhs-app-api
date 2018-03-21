namespace NHSOnline.Backend.Worker.Suppliers
{
    public interface ISystemProvider
    {
        IIm1ConnectionService GetIm1ConnectionService();
    }
}