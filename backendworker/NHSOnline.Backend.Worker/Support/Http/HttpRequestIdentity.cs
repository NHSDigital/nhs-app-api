using System;

namespace NHSOnline.Backend.Worker.Support.Http
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
                $"Provider:{Provider} - RequestMethod:{Method} - RequestUrl:{RequestUrl} - RequestIdentifier:{Identifier} ";
        }
    }
}