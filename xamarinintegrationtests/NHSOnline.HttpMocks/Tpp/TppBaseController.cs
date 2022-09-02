using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Tpp
{
    public class TppBaseController: Controller
    {
        private readonly IPatients _patients;

        public TppBaseController(IPatients patients)
        {
            _patients = patients;
        }

        protected virtual IPatients GetPatients()
        {
            return _patients;
        }

        protected IActionResult ReturnXmlResult(string patientId, string serializedXmlResponse)
        {
            Request.HttpContext.Response.Headers.Add("Suid", patientId);
            return new ContentResult
            {
                Content = serializedXmlResponse,
                ContentType = "text/xml"
            };
        }
    }
}