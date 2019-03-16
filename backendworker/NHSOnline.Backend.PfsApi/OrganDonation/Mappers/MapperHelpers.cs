using System.Collections.Generic;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal static class MapperHelpers 
    {
        internal static List<T> MapList<T>(T entry) => new List<T> { entry };

        internal static CodeableConcept MapConcept(string system, string code)
        {
            return new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding { System = system, Code = code }
                }
            };
        }
    }
}