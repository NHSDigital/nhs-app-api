using System;

namespace NHSOnline.Backend.Worker.Brothermailer
{
    public interface IBrothermailerConfig
    {         
        Uri BrothermailerBaseUrl { get; }
        string BrothermailerAddressBookId { get; }
        string BrothermailerSig { get; }
        string BrothermailerUserId { get; }
     
    }
}