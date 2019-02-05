using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface ITppDetailedTestResultMapper
    {       
        TestResultResponse Map(TestResultsViewReply testResultsViewReply);  
    }
}