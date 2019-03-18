using System;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Models
{
    public enum FaithDeclaration
    {
        [EnumMember(Value = "")]
        None = 0,
        NotStated,
        Yes,
        No
    }
}