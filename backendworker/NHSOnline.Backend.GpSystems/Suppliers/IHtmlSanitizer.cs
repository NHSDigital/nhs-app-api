namespace NHSOnline.Backend.GpSystems.Suppliers
{
    public interface IHtmlSanitizer
    {
        string SanitizeHtml(string html);
    }
}