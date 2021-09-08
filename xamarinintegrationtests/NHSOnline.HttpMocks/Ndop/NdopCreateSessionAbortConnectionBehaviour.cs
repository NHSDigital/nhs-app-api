using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Ndop
{
    public sealed class NdopCreateSessionAbortConnectionBehaviour : INdopCreateSessionBehaviour
    {
        public IActionResult Behave(string token, Dictionary<string,string> tokenContent, HttpRequest request,Func<string,object, ViewResult> view)
        {
            request.HttpContext.Abort();
            throw new InvalidOperationException("Connection should have been aborted at this point");
        }
    }
}