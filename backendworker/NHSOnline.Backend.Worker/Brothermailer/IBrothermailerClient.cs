using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Brothermailer.Models;

namespace NHSOnline.Backend.Worker.Brothermailer
{
    public interface IBrothermailerClient
    {
        Task<BrothermailerClient.BrothermailerApiObjectResponse>
            SendEmailAddress(BrothermailerRequest brothermailerRequest);
    }
}