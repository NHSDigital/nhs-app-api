using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Models.Patient;

namespace NHSOnline.Backend.Worker.Suppliers
{

    public interface INhsNumberProvider
    {
        Task<IEnumerable<PatientNhsNumber>> GetNhsNumbersAsync(string connectionToken, string odsCode);
    }
}
