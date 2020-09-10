using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Demographics.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Demographics
{
    internal class DemographicsResultVisitor : IDemographicsResultVisitor<IActionResult>
    {

        private readonly IMapper<DemographicsResult.Success, SuccessfulDemographicsResult> _resultMapper;
        
        public DemographicsResultVisitor(
            IMapper<DemographicsResult.Success, SuccessfulDemographicsResult> resultMapper
        )
        {
            _resultMapper = resultMapper;
        }
        
        public IActionResult Visit(DemographicsResult.Forbidden result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(DemographicsResult.Success result)
        {
            var mappedResult = _resultMapper.Map(result);
            return new OkObjectResult(mappedResult);
        }

        public IActionResult Visit(DemographicsResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(DemographicsResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}