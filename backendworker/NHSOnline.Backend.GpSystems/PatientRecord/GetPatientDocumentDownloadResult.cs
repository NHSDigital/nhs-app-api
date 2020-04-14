using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.GpSystems.PatientRecord
{
    public abstract class GetPatientDocumentDownloadResult
    {
        public abstract T Accept<T>(IPatientDocumentDownloadResultVisitor<T> visitor);

        public class Success : GetPatientDocumentDownloadResult
        {
            public FileContentResult Response { get; }

            public Success(FileContentResult response)
            {
                Response = response;
            }

            public override T Accept<T>(IPatientDocumentDownloadResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }
        
        public class BadGateway : GetPatientDocumentDownloadResult
        {
            public override T Accept<T>(IPatientDocumentDownloadResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
