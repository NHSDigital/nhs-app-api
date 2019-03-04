namespace NHSOnline.Backend.Worker.GpSearch.Models
{
    public class ContactInformation
    {
        public ResponseEnums.OrganisationContactMethodType OrganisationContactMethodType { get; set; }

        public string OrganisationContactValue { get; set; }
    }
}
