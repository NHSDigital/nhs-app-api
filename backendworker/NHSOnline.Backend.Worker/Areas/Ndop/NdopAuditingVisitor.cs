using NHSOnline.Backend.Worker.Areas.Ndop.Models;
using NHSOnline.Backend.Worker.Ndop;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Ndop
{
    public class NdopAuditingVisitor : INdopResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.ViewPatientRecordAuditTypeResponse;

        public NdopAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }
        
        public object Visit(GetNdopResult.SuccessfullyRetrieved result)
        {
            _auditor.Audit(AuditType, "Ndop Token successfully retrieved");
            
            return null;
        }

        public object Visit(GetNdopResult.Unsuccessful result)
        {
            _auditor.Audit(AuditType, "Error: Unsuccessful");
            return null;
        }
    }
}