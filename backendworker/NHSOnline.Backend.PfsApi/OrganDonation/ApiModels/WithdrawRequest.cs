using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.PfsApi.OrganDonation.ApiModels
{
    internal class WithdrawRequest : RegistrationBase
    {
        [Required]
        public CodeableConcept WithdrawReason { get; set; }
    }
}