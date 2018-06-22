namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class AppointmentSlot
    {
        public int SlotId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string SlotTypeName { get; set; }
    }
}