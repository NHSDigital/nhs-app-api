using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Ndop
{
    public interface INdopCreateSessionBehaviour
    {
        IActionResult Behave(string token, Dictionary<string,string> tokenContent, HttpRequest request,Func<string,object, ViewResult> view);
    }
}