namespace NHSOnline.Backend.Support
{
    public sealed class VersionTag
    {
        public VersionTag(string api, string web, string native)
        {
            Web = web;
            Native = native;
            Api = api;
        }

        public string Web { get; }
        public string Native { get; }
        public string Api { get; }
    }
}
