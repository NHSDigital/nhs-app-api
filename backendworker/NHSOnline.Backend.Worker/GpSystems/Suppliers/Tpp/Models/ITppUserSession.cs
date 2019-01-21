namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    public interface ITppUserSession
    {
        string PatientId { get; }
        string UnitId { get; }
        string OnlineUserId { get; }
    }
}
