using System;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Session;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public interface IFakeUser
    {
        string NhsNumber { get; }
        string GivenName { get; }
        string FamilyName { get; }
        DateTime DateOfBirth { get; }
        string Sex { get; }
        DemographicsAddress AddressParts { get; }
        string Address { get; }
        string Name { get;  }

        IAppointmentsBehaviour AppointmentsBehaviour { get; }
        IAppointmentSlotsBehaviour AppointmentSlotsBehaviour { get; }

        IDemographicsBehaviour DemographicsBehaviour { get; }

        IIm1ConnectionBehaviour Im1ConnectionBehaviour { get; }

        ILinkageBehaviour LinkageBehaviour { get; }

        IPatientRecordBehaviour PatientRecordBehaviour { get; }

        ICourseBehaviour CourseBehaviour { get; }
        IPrescriptionBehaviour PrescriptionBehaviour { get; }

        ISessionBehaviour SessionBehaviour { get; }
        ISessionExtendBehaviour SessionExtendBehaviour { get; }
    }
}