using System.Collections.Generic;

namespace NHSOnline.Backend.Support.Sanitization
{
    public interface IHtmlSanitizer
    {
        string SanitizeHtml(string html, HashSet<string> whitelist);
    }
}