using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Host("ers.stubs.local.bitraft.io")]
    public class ErsController : Controller
    {
        [HttpGet("nhslogin")]
        public IActionResult NhsLogin()
        {
            return Content($@"
                <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1, shrink-to-fit=no'>
                        <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css' integrity='sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk' crossorigin='anonymous'>
                        <title>eRS Web Integration</title>
                        <script type='text/javascript' src='http://web.local.bitraft.io:3000/js/v1/nhsapp.js'></script>
                        <script>
                            // Appium on iOS doesn't like us deleting the webview during the click event
                            function onClickNHSAppAppointments() {{
                                setTimeout(function() {{ nhsapp.navigation.goToPage(nhsapp.navigation.AppPage.APPOINTMENTS); }}, 1);
                                return false;
                            }}
                        </script>
                    </head>
                    <body>
                        <div class='jumbotron'>
                            <h1 class='display-4'>eRS</h1>
                        </div>
                        <div class='container'>
                            <ul class='list-group'>
                                <li class='list-group-item'><a href='/Internal/Page.html'>Internal Page</a></li>
                                <li class='list-group-item'><a href='http://auth.nhslogin.stubs.local.bitraft.io:8080/citizenid/authorize'>NHS Login</a></li>
                                <li class='list-group-item'><a href='http://stubs.local.bitraft.io:8080/nhsuk/covid'>Covid</a></li>
                                <li class='list-group-item'><a href='#' onclick='return onClickNHSAppAppointments()'>NHS App Appointments</a></li>
                            </ul>
                            <h2 class='display-5'>Headers</h2>
                            <ul class='list-group'>
                                <li class='list-group-item'>Host: {Request.Host}</li>
                                <li class='list-group-item'>Path: {Request.Path}</li>
                            </ul>
                            <h2 class='display-5'>Query String</h2>
                            <ul class='list-group'>
                                {string.Join(string.Empty, Request.Query.Select(q => $@"<li class='list-group-item'>{q.Key}: {q.Value}"))}
                            </ul>
                            <h2 class='display-5'>Headers</h2>
                            <ul class='list-group'>
                                {string.Join(string.Empty, Request.Headers.Select(q => $@"<li class='list-group-item'>{q.Key}: {q.Value}"))}
                            </ul>
                        </div>
                    </body>
                </html>",
                "text/html");
        }

        [HttpGet("Internal/Page.html")]
        public IActionResult InternalPage()
        {
            return Content($@"
                <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1, shrink-to-fit=no'>
                        <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css' integrity='sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk' crossorigin='anonymous'>
                        <title>eRS Web Integration Internal Page</title>
                        <script type='text/javascript' src='http://web.local.bitraft.io:3000/js/v1/nhsapp.js'></script>
                    </head>
                    <body>
                        <div class='jumbotron'>
                            <h1 class='display-4'>eRS Internal Page</h1>
                        </div>
                        <div class='container'>
                            <ul class='list-group'>
                                <li class='list-group-item'><a href='/NhsLogin'>Back</a></li>
                                <li class='list-group-item'><a href='#'onclick=' nhsapp.navigation.goToPage(nhsapp.navigation.AppPage.APPOINTMENTS);'>NHS App Appointments</a></li>
                            </ul>
                            <h2 class='display-5'>Headers</h2>
                            <ul class='list-group'>
                                <li class='list-group-item'>Host: {Request.Host}</li>
                                <li class='list-group-item'>Path: {Request.Path}</li>
                            </ul>
                            <h2 class='display-5'>Query String</h2>
                            <ul class='list-group'>
                                {string.Join(string.Empty, Request.Query.Select(q => $@"<li class='list-group-item'>{q.Key}: {q.Value}"))}
                            </ul>
                            <h2 class='display-5'>Headers</h2>
                            <ul class='list-group'>
                                {string.Join(string.Empty, Request.Headers.Select(q => $@"<li class='list-group-item'>{q.Key}: {q.Value}"))}
                            </ul>
                        </div>
                    </body>
                </html>",
                "text/html");
        }
    }
}
