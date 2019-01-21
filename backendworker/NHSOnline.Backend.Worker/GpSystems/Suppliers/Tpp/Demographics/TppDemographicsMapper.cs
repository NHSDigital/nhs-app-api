using System;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Demographics
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