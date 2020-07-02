using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics
{
    [FakeGpAreaBehaviour(Behaviour.Default)]
    public class DefaultDemographicsAreaBehaviour : IDemographicsAreaBehaviour
    {
        public Task<DemographicsResult> GetDemographics(GpLinkedAccountModel gpLinkedAccountModel,
            FakeUser fakeUser)
        {
            var response = GetDemographicsResponse(fakeUser);
            return Task.FromResult<DemographicsResult>(new DemographicsResult.Success(response));
        }

        private static DemographicsResponse GetDemographicsResponse(FakeUser user)
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