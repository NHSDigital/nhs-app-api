using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics
{
    public class DefaultDemographicsBehaviour : IDemographicsBehaviour
    {
        public Task<DemographicsResult> GetDemographics(GpLinkedAccountModel gpLinkedAccountModel,
            IFakeUser fakeUser)
        {
            var response = GetDemographicsResponse(fakeUser);
            return Task.FromResult<DemographicsResult>(new DemographicsResult.Success(response));
        }

        private static DemographicsResponse GetDemographicsResponse(IFakeUser user)
        {
            var response = new DemographicsResponse
            {
                Address = user.Address,
                AddressParts = user.AddressParts,
                PatientName = user.Name,
                NameParts = new DemographicsName
                {
                    Given = user.GivenName,
                    Surname = user.FamilyName
                },
                DateOfBirth = user.DateOfBirth,
                NhsNumber = user.NhsNumber.FormatToNhsNumber(),
                Sex = user.Sex
            };
            return response;
        }
    }
}