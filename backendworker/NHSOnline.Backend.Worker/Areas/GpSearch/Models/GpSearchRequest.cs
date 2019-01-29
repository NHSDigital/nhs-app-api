using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.Worker.Areas.GpSearch.Models
{
    public class GpSearchRequest
    {
        [Required]
        public string SearchTerm { get; set; }
    }
}