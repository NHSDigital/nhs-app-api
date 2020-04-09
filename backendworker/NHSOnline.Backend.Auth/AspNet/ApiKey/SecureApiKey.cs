using System;

namespace NHSOnline.Backend.Auth.AspNet.ApiKey
{
    public class SecureApiKey
    {
        public SecureApiKey(string owner, string key)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public string Owner { get; }
        public string Key { get; }
    }
}
