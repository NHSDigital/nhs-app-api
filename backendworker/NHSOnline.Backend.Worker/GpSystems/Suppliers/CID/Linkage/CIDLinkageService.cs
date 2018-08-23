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

        public Task<LinkageResult> GetLinkageKey(string nhsNumber, string odsCode, string identityToken)
        {
            if (!OdsCode.Equals(odsCode, StringComparison.Ordinal))
            {
                return Task.FromResult((LinkageResult)new LinkageResult.NotFoundErrorRetrievingNhsUser());
            }

            var existingPatientlinkage = GetLinkageData.ExistingPatientLinkage
                .FirstOrDefault(x => string.Equals(x.NhsNumber, nhsNumber, StringComparison.Ordinal));

            if (existingPatientlinkage != null)
            {
                return Task.FromResult(
                    (LinkageResult)new LinkageResult.SuccessfullyRetrieved(
                        existingPatientlinkage.LinkageResponse));
            }

            var expiredPatientLinkage =
                GetLinkageData.ExpiredPatientLinkage.FirstOrDefault(x => string.Equals(x.NhsNumber, nhsNumber, StringComparison.Ordinal));
            
            if (GetLinkageData.BadGatewayPatient.NhsNumber.Equals(nhsNumber, StringComparison.Ordinal) ||
                GetLinkageData.TimeOutPatient.NhsNumber.Equals(nhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((LinkageResult)new LinkageResult.SupplierSystemUnavailable());
            }

            return Task.FromResult((LinkageResult)new LinkageResult.NotFoundErrorRetrievingNhsUser());
        }

        public Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            if (!OdsCode.Equals(createLinkageRequest.OdsCode, StringComparison.Ordinal))
            {
                return Task.FromResult((LinkageResult)new LinkageResult.NotFoundErrorCreatingNhsUser());
            }

            if (CreateLinkageData.CidRequestedData1.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((LinkageResult)new LinkageResult.SuccessfullyRetrieved
                    (CreateLinkageData.CidRequestedData1.LinkageResponse));
            }

            if (CreateLinkageData.CidRequestedData2.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((LinkageResult)new LinkageResult.SuccessfullyRetrieved
                    (CreateLinkageData.CidRequestedData2.LinkageResponse));
            }

            if (CreateLinkageData.CidRequestedData3.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((LinkageResult)new LinkageResult.SuccessfullyRetrieved
                    (CreateLinkageData.CidRequestedData3.LinkageResponse));
            }

            if (CreateLinkageData.ValidPatient.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((LinkageResult)new LinkageResult.SuccessfullyRetrieved
                    (CreateLinkageData.ValidPatient.LinkageResponse));
            }

            if (CreateLinkageData.NotFoundPatient.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((LinkageResult)new LinkageResult.NotFoundErrorCreatingNhsUser());
            }

            if (CreateLinkageData.ConflictPatient.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((LinkageResult)new LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount());
            }

            if (CreateLinkageData.BadGatewayPatient.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal) ||
                CreateLinkageData.TimeoutPatient.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal))
            {
                return Task.FromResult((LinkageResult)new LinkageResult.SupplierSystemUnavailable());
            }

            return Task.FromResult((LinkageResult)new LinkageResult.NotFoundErrorCreatingNhsUser());
        }
    }
}