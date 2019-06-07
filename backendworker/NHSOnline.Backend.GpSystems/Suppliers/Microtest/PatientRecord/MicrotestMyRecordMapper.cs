using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    public class MicrotestMyRecordMapper : IMicrotestMyRecordMapper
    {
        public MyRecordResponse Map(PatientRecordGetResponse patientRecordGetResponse)
        {
            if (patientRecordGetResponse == null)
            {
                throw new System.ArgumentNullException(nameof(patientRecordGetResponse));
            }

            var myRecordResponse = new MyRecordResponse();

            MapAllergies(myRecordResponse, patientRecordGetResponse.AllergyData);

            return myRecordResponse;
        }


        private static void MapAllergies(MyRecordResponse myRecordResponse, AllergyData allergyData)
        {
            if (allergyData != null)
            {
                var allergies = allergyData.Allergies.Where(o => !string.IsNullOrEmpty(o.Severity));

                myRecordResponse.Allergies.Data = allergies
                    .Select(x => new AllergyItem
                    {
                        Name = x.Description,
                        Date = x.StartDate != null
                            ? new MyRecordDate
                            {
                                Value = DateTime.TryParse(x.StartDate, out var eventDate)
                                    ? eventDate
                                    : (DateTimeOffset?) null,
                                DatePart = x.StartDate
                            }
                            : null
                    })
                    .OrderByDescending(o => o.Date?.Value.GetValueOrDefault())
                    .ToList();

                myRecordResponse.HasSummaryRecordAccess = IsAny(myRecordResponse.Allergies.Data);
            }
        }

        private static bool IsAny<T>(IEnumerable<T> data)
        {
            return data != null && data.Any();
        }
    }
}