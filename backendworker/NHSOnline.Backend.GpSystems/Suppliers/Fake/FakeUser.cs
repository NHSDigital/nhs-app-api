using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Session;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public abstract class FakeUser : IFakeUser
    {
        private readonly IServiceProvider _serviceProvider;

        protected FakeUser(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public abstract Guid Id { get; }
        public abstract string EmailAddress { get; }
        public abstract string NhsNumber { get ; }
        public abstract string OdsCode { get; }
        public abstract string GivenName { get; }
        public abstract string FamilyName { get; }
        public abstract DateTime DateOfBirth { get; }
        public abstract string Sex { get; }

        public virtual string Name => $"{GivenName} {FamilyName}";

        public virtual IEnumerable<string> LinkedAccountsNhsNumbers => Enumerable.Empty<string>();

        public virtual DemographicsAddress AddressParts => new DemographicsAddress
        {
            HouseName = "Richmond House",
            NumberStreet = "79 Whitehall",
            Town = "London",
            Postcode = "SW1A 2NL"
        };

        public virtual string Address
        {
            get
            {
                var addressParts = new List<string>
                {
                    AddressParts.HouseName,
                    AddressParts.NumberStreet,
                    AddressParts.Village,
                    AddressParts.Town,
                    AddressParts.County,
                    AddressParts.Postcode
                };

                return string.Join(", ", addressParts.Where(part => !string.IsNullOrEmpty(part)));
            }
        }

        public virtual IAppointmentsBehaviour AppointmentsBehaviour =>
            _serviceProvider.GetService<DefaultAppointmentsBehaviour>();

        public virtual IAppointmentSlotsBehaviour AppointmentSlotsBehaviour =>
            _serviceProvider.GetService<DefaultAppointmentSlotsBehaviour>();

        public virtual IDemographicsBehaviour DemographicsBehaviour =>
            _serviceProvider.GetService<DefaultDemographicsBehaviour>();

        public virtual IIm1ConnectionBehaviour Im1ConnectionBehaviour =>
            _serviceProvider.GetService<DefaultIm1ConnectionBehaviour>();

        public virtual ILinkageBehaviour LinkageBehaviour =>
            _serviceProvider.GetService<DefaultLinkageBehaviour>();

        public virtual ILinkedAccountsBehaviour LinkedAccountsBehaviour =>
            _serviceProvider.GetService<DefaultLinkedAccountsBehaviour>();

        public virtual IPatientRecordBehaviour PatientRecordBehaviour =>
            _serviceProvider.GetService<DefaultPatientRecordBehaviour>();

        public virtual ICourseBehaviour CourseBehaviour =>
            _serviceProvider.GetService<DefaultCourseBehaviour>();
        public virtual IPrescriptionBehaviour PrescriptionBehaviour =>
            _serviceProvider.GetService<DefaultPrescriptionBehaviour>();

        public virtual ISessionBehaviour SessionBehaviour =>
            _serviceProvider.GetService<DefaultSessionBehaviour>();
        public virtual ISessionExtendBehaviour SessionExtendBehaviour =>
            _serviceProvider.GetService<DefaultSessionExtendBehaviour>();
    }
}