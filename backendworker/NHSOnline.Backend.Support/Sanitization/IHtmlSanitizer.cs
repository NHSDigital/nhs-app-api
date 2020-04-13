namespace NHSOnline.Backend.Support.Sanitization
{
    public interface IHtmlSanitizer
    {
        string SanitizeHtml(string html);

        string GetBodyContent(string html);
    }
}