using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.Brothermailer.Models;

namespace NHSOnline.Backend.PfsApi.Brothermailer
{
    public interface IBrothermailerClient
    {
        Task<BrothermailerClient.BrothermailerApiObjectResponse>
            SendEmailAddress(BrothermailerRequest brothermailerRequest);
    }
}