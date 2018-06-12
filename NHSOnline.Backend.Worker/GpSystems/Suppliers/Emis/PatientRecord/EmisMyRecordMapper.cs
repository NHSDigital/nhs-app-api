using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisMyRecordMapper : IEmisMyRecordMapper
    {
        private readonly ILogger<EmisMyRecordMapper> _logger;

        public EmisMyRecordMapper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EmisMyRecordMapper>();
        }
        
        public MyRecordResponse Map(AllergyRequestsGetResponse allergiesGetResponse)
        {
            if (allergiesGetResponse == null)
            {
                throw new ArgumentNullException(nameof(allergiesGetResponse));
            }

            var allergies = new Allergies();
            
            if (allergiesGetResponse.MedicalRecord != null)
            {
                _logger.LogInformation("MedicalRecord exists");
                
                var medicalRecord = allergiesGetResponse.MedicalRecord;

                allergies.Data = (medicalRecord.Allergies ?? Enumerable.Empty<AllergyResponse>()).Select(x =>
                    new AllergyItem
                    {
                        Name = x.Term,
                        Date = x.AvailabilityDateTime
                    });
                _logger.LogInformation("Allergies count: " + allergies.Data.Count());
            }
            
            var result = new MyRecordResponse
            {
                Allergies = allergies
            };

            return result;
        }
    }
}