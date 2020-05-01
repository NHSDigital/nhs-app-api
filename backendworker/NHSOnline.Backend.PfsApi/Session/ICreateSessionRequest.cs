using System;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.PfsApi.Session
{
    public interface ICreateSessionRequest
    {
        string AuthCode { get; }
        string CodeVerifier { get; }
        Uri RedirectUrl { get; }
        string CsrfToken { get; }
        HttpContext HttpContext { get; }
    }
}