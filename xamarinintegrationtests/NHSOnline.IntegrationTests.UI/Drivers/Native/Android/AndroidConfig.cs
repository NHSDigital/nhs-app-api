using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    internal sealed class AndroidConfig
    {
        public string App { get; set; } = $"{Dns.GetHostName()}-android";

        public string? PlayStorePassword { get; set; }

        public string PlayStoreUser { get; set; } = "nhsappbrowserstack@gmail.com";

        public Dictionary<string, string> GoogleCredentials()
        {
            return  new() {{ "username", PlayStoreUser },{ "password", GetUserPassword() }};
        }

        private string GetUserPassword()
        {
            if (PlayStorePassword != null)
            {
                return PlayStorePassword;
            }

            var keyFilePath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".nhsonline",
                "secrets",
                "browserstack_playstore_user_pass");
            return File.ReadAllText(keyFilePath);
        }
    }
}