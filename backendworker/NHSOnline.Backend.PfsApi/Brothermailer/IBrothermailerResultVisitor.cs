using NHSOnline.Backend.PfsApi.Brothermailer.Models;

namespace NHSOnline.Backend.PfsApi.Brothermailer
{
    public interface IBrothermailerResultVisitor<out T>
    {
        T Visit(BrothermailerResult.Success result);
        T Visit(BrothermailerResult.InternalServerError result);
        T Visit(BrothermailerResult.BadGateway result);
        T Visit(BrothermailerResult.BadRequest result);      
    }
}