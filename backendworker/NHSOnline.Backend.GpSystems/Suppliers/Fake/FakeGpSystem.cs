using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    [SuppressMessage("NDepend", "ND1400:AvoidNamespacesMutuallyDependent",
        Justification = "GpSystem responsibility is pointing to concrete classes in child namespaces.")]
    [SuppressMessage("Microsoft.Naming", "CA1024", Justification = "Methods are needed to match interface definition.")]
    public class FakeGpSystem : IGpSystem
    {
        private readonly IServiceProvider _serviceProvider;

        public FakeGpSystem(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Supplier Supplier => Supplier.Fake;
        public IAppointmentsService GetAppointmentsService() =>
            _serviceProvider.GetService<FakeAppointmentsService>();

        public IAppointmentSlotsService GetAppointmentSlotsService() =>
            _serviceProvider.GetService<FakeAppointmentSlotsService>();

        public IAppointmentsValidationService GetAppointmentsValidationService() =>
            _serviceProvider.GetService<FakeAppointmentsValidationService>();

        public ICourseService GetCourseService() =>
            _serviceProvider.GetService<FakeCourseService>();

        public IDemographicsService GetDemographicsService() =>
            _serviceProvider.GetService<FakeDemographicsService>();

        public IIm1ConnectionService GetIm1ConnectionService() =>
            _serviceProvider.GetService<FakeIm1ConnectionService>();

        public IPrescriptionService GetPrescriptionService() =>
            _serviceProvider.GetService<FakePrescriptionService>();

        public IPrescriptionValidationService GetPrescriptionValidationService() =>
            _serviceProvider.GetService<FakePrescriptionValidationService>();

        public ISessionService GetSessionService() =>
            _serviceProvider.GetService<FakeSessionService>();

        public ISessionExtendService GetSessionExtendService() =>
            _serviceProvider.GetService<FakeSessionExtendService>();

        public ITokenValidationService GetTokenValidationService() =>
            _serviceProvider.GetService<FakeTokenValidationService>();

        public IPatientRecordService GetPatientRecordService() =>
            _serviceProvider.GetService<FakePatientRecordService>();

        public ILinkageService GetLinkageService() =>
            _serviceProvider.GetService<FakeLinkageService>();

        public ILinkageValidationService GetLinkageValidationService() =>
            _serviceProvider.GetService<FakeLinkageValidationService>();

        public ILinkedAccountsService GetLinkedAccountsService() =>
            _serviceProvider.GetService<FakeLinkedAccountsService>();

        public IPatientMessagesService GetPatientMessagesService() =>
            throw new NotImplementedException();

        public IRecreateSessionMapperService GetRecreateSessionMapperService() =>
            throw new NotImplementedException();

        public bool SupportsLinkedAccounts => true;
    }
}