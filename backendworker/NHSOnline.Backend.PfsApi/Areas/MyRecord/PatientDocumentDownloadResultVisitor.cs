using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    public class PatientDocumentDownloadResultVisitor: IPatientDocumentDownloadResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetPatientDocumentDownloadResult.Success result)
        {
            return result.Response;
        }

        public IActionResult Visit(GetPatientDocumentDownloadResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}