using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    [FakeGpAreaBehaviour(Behaviour.Default)]
    public class DefaultPrescriptionAreaBehaviour : IPrescriptionAreaBehaviour
    {

        private FilteringCounts _filteringCounts = new FilteringCounts
        {
            ReceivedCount = 1,
            ReturnedCount = 0,
            ReceivedRepeatsCount = 1,
            ExcessRepeatsCount = 0,
        };

        private PrescriptionListResponse _prescriptionsListResponse = new PrescriptionListResponse
        {
            Prescriptions = new List<PrescriptionItem>
            {
                new PrescriptionItem
                {
                    Courses = new List<CourseEntry>
                    {
                        new CourseEntry
                        {
                            CourseId = "123",
                        }
                    },
                    OrderDate = new DateTimeOffset(),
                    OrderedBy = "GP",
                    Status = Status.Approved
                }
            },
            Courses = new List<Course>
            {
                new Course
                {
                    Id = "123",
                    Name = "Medication 1",
                    Details = "Take 1 on days starting with 'T'"
                }
            }
        };

        public async Task<GetPrescriptionsResult> GetPrescriptions(GpLinkedAccountModel gpLinkedAccountModel,
            DateTimeOffset? fromDate = null,  DateTimeOffset? toDate = null)
        {
            return await Task.FromResult<GetPrescriptionsResult>(
                new GetPrescriptionsResult.Success(_prescriptionsListResponse, _filteringCounts));
        }

        public async Task<OrderPrescriptionResult> OrderPrescription(GpLinkedAccountModel gpLinkedAccountModel,
            RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            return await Task.FromResult<OrderPrescriptionResult>(new OrderPrescriptionResult.Success());
        }
    }
}