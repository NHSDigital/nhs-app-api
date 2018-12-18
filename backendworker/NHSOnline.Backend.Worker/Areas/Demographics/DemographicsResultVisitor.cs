using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Areas.Demographics.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.Areas.Demographics
{
    public class DemographicsResultVisitor : IDemographicsResultVisitor<IActionResult>
    {

        private readonly IMapper<DemographicsResult.SuccessfullyRetrieved, SuccessfulDemographicsResult> _resultMapper;
        
        public DemographicsResultVisitor(
            IMapper<DemographicsResult.SuccessfullyRetrieved, SuccessfulDemographicsResult> resultMapper
        )
        {
            _resultMapper = resultMapper;
        }
        
        public IActionResult Visit(DemographicsResult.UserHasNoAccess result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(DemographicsResult.SuccessfullyRetrieved result)
        {
            var mappedResult = _resultMapper.Map(result);
            return new OkObjectResult(mappedResult);
        }

        public IActionResult Visit(DemographicsResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(DemographicsResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(DemographicsResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}