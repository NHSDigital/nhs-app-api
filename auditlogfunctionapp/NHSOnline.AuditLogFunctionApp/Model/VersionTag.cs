namespace NHSOnline.AuditLogFunctionApp.Model
{
    public class VersionTag
    {
        public string Web { get; set; }
        public string Native { get; set; }
        public string Api { get; set; }

        public VersionTag(string api, string web, string native)
        {
            Web = web;
            Native = native;
            Api = api;
        }
    }
}
