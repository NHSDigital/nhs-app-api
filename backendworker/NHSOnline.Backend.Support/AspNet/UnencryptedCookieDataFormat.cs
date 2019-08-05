using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace NHSOnline.Backend.Support.AspNet
{
    public class UnencryptedCookieDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly TicketSerializer _ticketSerializer;

        public UnencryptedCookieDataFormat()
        {
            _ticketSerializer = new TicketSerializer();
        }

        public string Protect(AuthenticationTicket data)
        {
            return Encoding.UTF8.GetString(_ticketSerializer.Serialize(data));
        }

        public string Protect(AuthenticationTicket data, string purpose)
        {
            return Protect(data);
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            return _ticketSerializer.Deserialize(Encoding.UTF8.GetBytes(protectedText));
        }

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            return Unprotect(protectedText);
        }
    }
}
