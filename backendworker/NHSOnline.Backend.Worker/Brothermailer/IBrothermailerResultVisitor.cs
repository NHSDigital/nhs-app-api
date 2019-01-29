using NHSOnline.Backend.Worker.Brothermailer.Models;

namespace NHSOnline.Backend.Worker.Brothermailer
{
    public interface IBrothermailerResultVisitor<out T>
    {
        T Visit(BrothermailerResult.SuccessfullyRetrieved result);
        T Visit(BrothermailerResult.Unsuccessful result);
        T Visit(BrothermailerResult.BrothermailerServiceUnavailable result);
        T Visit(BrothermailerResult.BadRequest result);      
    }
}