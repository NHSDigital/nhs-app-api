extern alias r4;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;
using r4::Hl7.Fhir.Model;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Mappers
{
    public interface ISecondaryCareSummaryMapper
    {
        SummaryResponse Map(Bundle bundle);
    }
}