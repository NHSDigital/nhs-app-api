using System.Collections.Generic;

namespace NHSOnline.HttpMocks.Domain
{
    public interface IPatients
    {
        Patient? LookupById(string id);
        Patient? LookupByNhsNumber(string nhsNumber);
        IEnumerable<Patient> All();
    }
}