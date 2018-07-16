using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.CID.Linkage
{
    public class CidLinkageService : ILinkageService
    {
        private const string OdsCode = "A29928";

        public async Task<GetLinkageResult> GetLinkageKey(string nhsNumber, string odsCode)
        {
            if (OdsCode != odsCode)
            {
                return new GetLinkageResult.NhsNumberNotFound();
            }

            var existingPatientlinkage = GetLinkageData.ExistingPatientLinkage
                .FirstOrDefault(x => x.NhsNumber == nhsNumber);

            if (existingPatientlinkage != null)
            {
                return new GetLinkageResult.SuccessfullyRetrieved(existingPatientlinkage.LinkageResponse);
            }
            
            var expiredPatientLinkage = 
                GetLinkageData.ExpiredPatientLinkage.FirstOrDefault(x => x.NhsNumber == nhsNumber);

            if (expiredPatientLinkage != null)
            {
                return new GetLinkageResult.LinkageKeyRevoked();
            }

            if (GetLinkageData.BadGatewayPatient.NhsNumber == nhsNumber || GetLinkageData.TimeOutPatient.NhsNumber == nhsNumber)
            {
                return new GetLinkageResult.SupplierSystemUnavailable();
            }
            
            return new GetLinkageResult.NhsNumberNotFound();
        }

        public async Task<CreateLinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            if (OdsCode != createLinkageRequest.OdsCode)
            {
                return new CreateLinkageResult.NhsNumberNotFound();
            }

            if (createLinkageRequest.NhsNumber == CreateLinkageData.ValidPatient.NhsNumber)
            {
                return new CreateLinkageResult.SuccessfullyRetrieved
                    (CreateLinkageData.ValidPatient.LinkageResponse);
            }
            
            if (createLinkageRequest.NhsNumber == CreateLinkageData.NotFoundPatient.NhsNumber)
            {
                return new CreateLinkageResult.NhsNumberNotFound();
            }

            if (createLinkageRequest.NhsNumber == CreateLinkageData.ConflictPatient.NhsNumber)
            {
                return new CreateLinkageResult.LinkageKeyAlreadyExists();
            }

            if (createLinkageRequest.NhsNumber == CreateLinkageData.BadGatewayPatient.NhsNumber || 
                createLinkageRequest.NhsNumber == CreateLinkageData.TimeoutPatient.NhsNumber)
            {
                return new CreateLinkageResult.SupplierSystemUnavailable();
            }

            return new CreateLinkageResult.NhsNumberNotFound();
        }
    }   
}