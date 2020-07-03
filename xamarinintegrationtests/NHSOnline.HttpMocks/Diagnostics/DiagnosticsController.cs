using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Diagnostics
{
    [Route("diagnostics")]
    public sealed class DiagnosticsController: Controller
    {
        [Route("cookies")]
        public IActionResult Cookies()
        {
            return Content(@"
<html>
    <body>
        <h1>Cookies</h1>
        <span id=""myId""><span>
        <script>
            document.getElementById('myId').innerHTML = listCookies()
            function listCookies() {
                var theCookies = document.cookie.split(';');
                var aString = '';
                for (var i = 1; i <= theCookies.length; i++)
                {
                    aString += i + ' ' + theCookies[i - 1] + ""\n"";
                }
                return aString;
            }
        </script>
    </body>
</html>",
                "text/html");
        }
    }
}
