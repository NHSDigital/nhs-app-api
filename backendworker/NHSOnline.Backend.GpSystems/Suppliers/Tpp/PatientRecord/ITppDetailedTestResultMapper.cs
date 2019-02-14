using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface ITppDetailedTestResultMapper
    {       
        TestResultResponse Map(TestResultsViewReply testResultsViewReply);  
    }
}