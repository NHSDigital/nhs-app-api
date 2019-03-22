using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.PfsApi.Areas.GpSearch.Models
{
    public class PharmacySearchRequest
    {
        [Required]
        public string postcode { get; set; }
    }
}