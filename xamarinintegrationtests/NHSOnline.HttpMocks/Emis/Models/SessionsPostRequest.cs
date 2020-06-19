using System.ComponentModel.DataAnnotations;

namespace NHSOnline.HttpMocks.Emis.Models
{
    public class SessionsPostRequest
    {
        [Required]
        public string? AccessIdentityGuid { get; set; }

        [Required]
        public string? NationalPracticeCode { get; set; }
    }
}
