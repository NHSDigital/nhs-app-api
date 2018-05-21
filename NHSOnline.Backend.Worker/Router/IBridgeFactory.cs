namespace NHSOnline.Backend.Worker.Router
{
    public interface IBridgeFactory
    {
        IBridge CreateBridge(SupplierEnum supplier);
    }
}