using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    internal class RepositoryGetConsentResponseVisitor : IRepositoryFindResultVisitor<TermsAndConditionsRecord, TermsAndConditionsFetchConsentResult>
    {
        private readonly ILogger<TermsAndConditionsService> _logger;
        private readonly IMapper<TermsAndConditionsRecord, ConsentResponse> _mapper;

        public RepositoryGetConsentResponseVisitor(ILogger<TermsAndConditionsService> logger,
            IMapper<TermsAndConditionsRecord, ConsentResponse> mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public TermsAndConditionsFetchConsentResult Visit(RepositoryFindResult<TermsAndConditionsRecord>.NotFound result)
        {
            _logger.LogInformation("No patient consent exists for terms and conditions");
            return new TermsAndConditionsFetchConsentResult.NoConsentFound(new ConsentResponse());
        }

        public TermsAndConditionsFetchConsentResult Visit(RepositoryFindResult<TermsAndConditionsRecord>.InternalServerError result)
        {
            return new TermsAndConditionsFetchConsentResult.InternalServerError();
        }

        public TermsAndConditionsFetchConsentResult Visit(RepositoryFindResult<TermsAndConditionsRecord>.RepositoryError result)
        {
            return new TermsAndConditionsFetchConsentResult.InternalServerError();
        }

        public TermsAndConditionsFetchConsentResult Visit(RepositoryFindResult<TermsAndConditionsRecord>.Found result)
        {
            var termsAndConditions = result.Records.First();
            var response = _mapper.Map(termsAndConditions);
            return new TermsAndConditionsFetchConsentResult.Success(response);
        }
    }
}