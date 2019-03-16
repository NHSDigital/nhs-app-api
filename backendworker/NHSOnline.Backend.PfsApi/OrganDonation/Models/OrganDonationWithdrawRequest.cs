using System;
using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Models
{
    public class OrganDonationWithdrawRequest
    {
        public string Identifier { get; set; }

        public string NhsNumber { get; set; }

        public Name Name { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string AddressFull { get; set; }

        public Address Address { get; set; }

        public string WithdrawReasonId { get; set; }
    }
}