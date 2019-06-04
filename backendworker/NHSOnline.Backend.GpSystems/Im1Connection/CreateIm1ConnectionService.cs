using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.CreateIm1Connection
{
       public class CreateIm1ConnectionService : ICreateIm1ConnectionService
    {
        private readonly ILogger<CreateIm1ConnectionService> _logger;
        private readonly IOdsCodeMassager _odsCodeMassager;

        public CreateIm1ConnectionService(
            ILogger<CreateIm1ConnectionService> logger,
            IOdsCodeMassager odsCodeMassager)
        {
            _logger = logger;
            _odsCodeMassager = odsCodeMassager;
        }
        
        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request,
            IGpSystem gpSystem)
        {
            try
            {
                _logger.LogEnter();
                
                request.OdsCode = _odsCodeMassager.CheckOdsCode(request.OdsCode);

                var im1ConnectionService = gpSystem.GetIm1ConnectionService();
                return await im1ConnectionService.Register(request);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}