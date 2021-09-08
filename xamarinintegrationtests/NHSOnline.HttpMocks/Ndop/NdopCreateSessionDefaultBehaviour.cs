using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Ndop
{
    public class NdopCreateSessionDefaultBehaviour : INdopCreateSessionBehaviour
    {
        public IActionResult Behave(string token, Dictionary<string,string> tokenContent, HttpRequest request,Func<string,object, ViewResult> view)
        {
            (string Token, Dictionary<string,string> tokenContent, Microsoft.AspNetCore.Http.HttpRequest Request) model =
                (token, tokenContent, request);

            return view("Ndop", model);
        }
    }
}