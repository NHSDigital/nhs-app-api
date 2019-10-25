using System;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts.Models
{
    public class LinkedAccountsConfigResponse
    {
        public Guid Id { get; set; }
        public bool HasLinkedAccounts { get; set; }
    }
}