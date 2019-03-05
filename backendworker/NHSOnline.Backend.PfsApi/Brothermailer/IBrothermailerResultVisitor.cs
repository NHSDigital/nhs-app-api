using NHSOnline.Backend.PfsApi.Brothermailer.Models;

namespace NHSOnline.Backend.PfsApi.Brothermailer
{
    public interface IBrothermailerResultVisitor<out T>
    {
        T Visit(BrothermailerResult.SuccessfullyRetrieved result);
        T Visit(BrothermailerResult.Unsuccessful result);
        T Visit(BrothermailerResult.BrothermailerServiceUnavailable result);
        T Visit(BrothermailerResult.BadRequest result);      
    }
}