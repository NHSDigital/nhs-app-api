using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models;
using NHSOnline.Backend.Support;
using Name = NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models.Name;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Mappers
{
    internal class OlcDemographicsMapper : IMapper<DemographicsResponse, OlcDemographics>
    {
        private readonly IMapper<string, DemographicsName, Name> _demographicsNameMapper;
        private readonly ILogger<OlcDemographicsMapper> _logger;

        public OlcDemographicsMapper(
            IMapper<string, DemographicsName, Name> demographicsNameMapper,
            ILogger<OlcDemographicsMapper> logger)
        {
            _demographicsNameMapper = demographicsNameMapper;
            _logger = logger;
        }

        public OlcDemographics Map(DemographicsResponse source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ValidateAndLog.ValidationOptions.ThrowError)
                .IsValid();

            return new OlcDemographics
            {
                AddressFull = source.Address,
                NameFull = source.PatientName,
                Name = MapName(source),
                NhsNumber = source.NhsNumber,
                DateOfBirth = source.DateOfBirth
            };
        }
        private static Name MapName(OlcDemographics demographics)
        {
            var name = demographics.Name != null
                ? new Name
                {
                    Title = demographics.Name.Title,
                    GivenName = demographics.Name.GivenName,
                    Surname = demographics.Name.Surname
                }
                : null;

            return name;
        }

        private Name MapName(DemographicsResponse demographicsResponse)
        {
            return _demographicsNameMapper.Map(demographicsResponse.PatientName, demographicsResponse.NameParts);
        }
    }
}
