using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Brothermailer.Models;

namespace NHSOnline.Backend.Worker.Brothermailer
{
    public interface IBrothermailerService
    {
        Task<BrothermailerResult> SendEmailAddress(BrothermailerRequest brothermailerRequest);
    }
}