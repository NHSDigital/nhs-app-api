using System;

namespace NHSOnline.Backend.PfsApi.Brothermailer
{
    public interface IBrothermailerConfig
    {         
        Uri BrothermailerBaseUrl { get; }
        string BrothermailerAddressBookId { get; }
        string BrothermailerSig { get; }
        string BrothermailerUserId { get; }
     
    }
}