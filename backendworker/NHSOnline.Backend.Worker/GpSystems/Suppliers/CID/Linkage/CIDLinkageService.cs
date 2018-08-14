using System;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.CID.Linkage
{
    public class CidLinkageService : ILinkageService
    {
        private const string OdsCode = "A29928";

        public Task<GetLinkageResult> GetLinkageKey(string nhsNumber, string odsCode)
        {
            if (!OdsCode.Equals(odsCode, StringComparison.Ordinal))
            {
                return Task.FromResult((GetLinkageResult)new GetLinkageResult.NhsNumberNotFound());
            }

            var existingPatientlinkage = GetLinkageData.ExistingPatientLinkage
                .FirstOrDefault(x => string.Equals(x.NhsNumber, nhsNumber, StringComparison.Ordinal));

            if (existingPatientlinkage != null)
            {
                return Task.FromResult(
                    (GetLinkageResult) new GetLinkageResult.SuccessfullyRetrieved(
                        existingPatientlinkage.LinkageResponse));
            }
            
            var expiredPatientLinkage = 
                GetLinkageData.ExpiredPatientLinkage.FirstOrDefault(x => string.Equals(x.NhsNumber, nhsNumber, StringComparison.Ordinal));

            if (expiredPatientLinkage != null)
            {
                return Task.FromResult((GetLinkageResult) new GetLinkageResult.LinkageKeyRevoked());
            }

            if (GetLinkageData.BadGatewayPatient.NhsNumber.Equals(nhsNumber, StringComparison.Ordinal) || 
                GetLinkageData.TimeOutPatient.NhsNumber.Equals(nhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((GetLinkageResult) new GetLinkageResult.SupplierSystemUnavailable());
            }
            
            return Task.FromResult((GetLinkageResult)new GetLinkageResult.NhsNumberNotFound());
        }

        public Task<CreateLinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            if (!OdsCode.Equals(createLinkageRequest.OdsCode, StringComparison.Ordinal))
            {
                return Task.FromResult((CreateLinkageResult)new CreateLinkageResult.NhsNumberNotFound());
            }
            
            if (CreateLinkageData.CidRequestedData1.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((CreateLinkageResult) new CreateLinkageResult.SuccessfullyRetrieved
                    (CreateLinkageData.CidRequestedData1.LinkageResponse));
            }
            
            if (CreateLinkageData.CidRequestedData2.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((CreateLinkageResult) new CreateLinkageResult.SuccessfullyRetrieved
                    (CreateLinkageData.CidRequestedData2.LinkageResponse));
            }
            
            if (CreateLinkageData.CidRequestedData3.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((CreateLinkageResult) new CreateLinkageResult.SuccessfullyRetrieved
                    (CreateLinkageData.CidRequestedData3.LinkageResponse));
            }

            if (CreateLinkageData.ValidPatient.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((CreateLinkageResult) new CreateLinkageResult.SuccessfullyRetrieved
                    (CreateLinkageData.ValidPatient.LinkageResponse));
            }
            
            if (CreateLinkageData.NotFoundPatient.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((CreateLinkageResult)new CreateLinkageResult.NhsNumberNotFound());
            }

            if (CreateLinkageData.ConflictPatient.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((CreateLinkageResult) new CreateLinkageResult.LinkageKeyAlreadyExists());
            }

            if (CreateLinkageData.BadGatewayPatient.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal) ||
                CreateLinkageData.TimeoutPatient.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((CreateLinkageResult) new CreateLinkageResult.SupplierSystemUnavailable());
            }

            return Task.FromResult((CreateLinkageResult) new CreateLinkageResult.NhsNumberNotFound());
        }
    }   
}