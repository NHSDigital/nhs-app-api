using System;

namespace NHSOnline.Backend.Support.Http
{
    public class HttpRequestIdentity
    {
        public string Provider { get; set; }
        public string Method { get; set; }
        public Uri RequestUrl { get; set; }
        public string Identifier { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Identifier))
                Identifier = null;

            return
                $"{nameof(Provider)}={Provider} UpStreamMethod={Method} UpStreamUrl={RequestUrl} UpStreamIdentifier={Identifier} ";
        }
    }
}