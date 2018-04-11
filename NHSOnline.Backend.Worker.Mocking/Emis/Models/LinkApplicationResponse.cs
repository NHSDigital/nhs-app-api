namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class LinkApplicationResponse
    {
        public string AccessIdentityGuid { get; set; }

        public LinkApplicationResponse(string accessIdentityGuid)
        {
            AccessIdentityGuid = accessIdentityGuid;
        }
    }
}
