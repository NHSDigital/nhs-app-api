namespace NHSOnline.Backend.Worker.Mocking.Models
{
    public class BodyEncoding
    {
        public BodyEncoding()
        {
            CodePage = 65001;
            EncodingName = "Unicode (UTF-8)";
            WebName = "utf-8";
        }

        public int CodePage { get; set; }
        public string EncodingName { get; set; }
        public string WebName { get; set; }
    }
}
