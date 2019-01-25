namespace NHSOnline.Backend.Worker.GpSystems.Suppliers
{
    public interface IHtmlSanitizer
    {
        string SanitizeHtml(string html);
    }
}