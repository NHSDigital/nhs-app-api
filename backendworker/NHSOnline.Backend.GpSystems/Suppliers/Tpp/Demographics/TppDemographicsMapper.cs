using System;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Demographics
{
    public class TppDemographicsMapper: ITppDemographicsMapper
    {
        public DemographicsResponse Map(PatientSelectedReply patientSelectedReply)
        {
            if (patientSelectedReply == null)
            {
                throw new ArgumentNullException(nameof(patientSelectedReply));
            }
            
            return new DemographicsResponse
            {
                PatientName = patientSelectedReply.Person?.PersonName.Name,
                DateOfBirth = patientSelectedReply.Person?.DateOfBirth,
                Sex = patientSelectedReply.Person?.Gender,
                NhsNumber = patientSelectedReply.Person?.NationalId.Value.FormatToNhsNumber(),
                Address = patientSelectedReply.Person?.Address?.Address,
            };
        }
    }
}