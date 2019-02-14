namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    public interface ITppUserSession
    {
        string PatientId { get; }
        string UnitId { get; }
        string OnlineUserId { get; }
    }
}
