using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Tpp.Models;

namespace NHSOnline.HttpMocks.Tpp
{
    [Route("Tpp")]
    public sealed class TppLogoffController : Controller
    {
        [Produces("application/xml")]
        [HttpPost]
        [TppTypeHeader("Logoff")]
        public Task<LogoffReply> Logoff()
        {
            return Task.FromResult(new LogoffReply() {Uuid = new Guid()});
        }
    }
}