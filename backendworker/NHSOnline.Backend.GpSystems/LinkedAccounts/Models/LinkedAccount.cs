using System;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts.Models
{
    public class LinkedAccount
    {
        public Guid Id { get; set; }

        public string GivenName { get; set; }

        public string Name { get; set; }
        
        public int? AgeMonths { get; set; }
        
        public int? AgeYears { get; set; }
        
        public string NhsNumber { get; set; }
    }
}
