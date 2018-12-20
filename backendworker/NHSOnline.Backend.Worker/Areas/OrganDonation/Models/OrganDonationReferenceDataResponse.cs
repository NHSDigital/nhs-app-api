using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation.Models
{
    public class OrganDonationReferenceDataResponse
    {
        public IEnumerable<SelectOption> Titles { get; set; } = new List<SelectOption>();

        public IEnumerable<SelectOption> Genders { get; set; } = new List<SelectOption>();

        public IEnumerable<SelectOption> Religions { get; set; } = new List<SelectOption>();

        public IEnumerable<SelectOption> Ethnicities { get; set; } = new List<SelectOption>();

        public IEnumerable<SelectOption> WithdrawReasons { get; set; } = new List<SelectOption>();
    }
}