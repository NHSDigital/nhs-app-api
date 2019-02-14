namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public interface IRepeat
    {
        string Drug { get; set; }
        
        string Dosage { get; set; }

        string Quantity { get; set; }
    }
}