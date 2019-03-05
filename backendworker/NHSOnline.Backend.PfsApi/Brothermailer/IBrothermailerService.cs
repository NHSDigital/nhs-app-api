using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.Brothermailer.Models;

namespace NHSOnline.Backend.PfsApi.Brothermailer
{
    public interface IBrothermailerService
    {
        Task<BrothermailerResult> SendEmailAddress(BrothermailerRequest brothermailerRequest);
    }
}