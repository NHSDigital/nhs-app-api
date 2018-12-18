using System;

namespace NHSOnline.Backend.Worker.CitizenId
{
    [Serializable]
    public class CitizenIdUserSession
    {
        public string AccessToken { get; set; }
    }
}