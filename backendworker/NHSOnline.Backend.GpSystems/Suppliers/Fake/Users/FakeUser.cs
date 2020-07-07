using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Session;
using NHSOnline.Backend.Repository;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users
{
    public class FakeUser : RepositoryRecord
    {
        public FakeUser()
        {
            Timestamp = DateTime.Now;

            LinkedAccountsNhsNumbers ??= new List<string>();
            Behaviours ??= new AreaBehaviours();
        }

        public string GroupName { get; set; }
        public string UserId { get; set; }
        public string EmailAddress { get; set; }
        public string NhsNumber { get; set; }
        public string OdsCode { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; }
        public AreaBehaviours Behaviours { get; set; }
        
        public IList<string> LinkedAccountsNhsNumbers { get; set; }

        [YamlIgnore]
        [BsonIgnore]
        public string Name => $"{GivenName} {FamilyName}";

        [YamlIgnore]
        [BsonIgnore]
        public Guid UserUuid => Guid.Parse(UserId);

        [YamlIgnore]
        [BsonIgnore]
        public DemographicsAddress AddressParts => new DemographicsAddress
        {
            HouseName = "Richmond House",
            NumberStreet = "79 Whitehall",
            Town = "London",
            Postcode = "SW1A 2NL"
        };

        [YamlIgnore]
        [BsonIgnore]
        public string Address
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

        [YamlIgnore]
        [BsonElement("_id")]
        public Guid Id { get; set; }

        [YamlIgnore]
        [BsonIgnore]
        public IServiceProvider ServiceProvider { get; set; }

        [YamlIgnore]
        [BsonIgnore]
        public IAppointmentsAreaBehaviour AppointmentsAreaBehaviour =>
            ServiceProvider?.ResolveAreaBehaviour<IAppointmentsAreaBehaviour>(
                Behaviours?.Appointments ?? Behaviour.Default
            );

        [YamlIgnore]
        [BsonIgnore]
        public IAppointmentSlotsAreaBehaviour AppointmentSlotsAreaBehaviour =>
            ServiceProvider?.ResolveAreaBehaviour<IAppointmentSlotsAreaBehaviour>(
                Behaviours?.AppointmentSlots ?? Behaviour.Default
            );

        [YamlIgnore]
        [BsonIgnore]
        public IDemographicsAreaBehaviour DemographicsAreaBehaviour =>
            ServiceProvider?.ResolveAreaBehaviour<IDemographicsAreaBehaviour>(
                Behaviours?.Demographics ?? Behaviour.Default
            );

        [YamlIgnore]
        [BsonIgnore]
        public IIm1ConnectionAreaBehaviour Im1ConnectionAreaBehaviour =>
            ServiceProvider?.ResolveAreaBehaviour<IIm1ConnectionAreaBehaviour>(
                Behaviours?.Im1Connection ?? Behaviour.Default
            );

        [YamlIgnore]
        [BsonIgnore]
        public ILinkageAreaBehaviour LinkageAreaBehaviour =>
            ServiceProvider?.ResolveAreaBehaviour<ILinkageAreaBehaviour>(
                Behaviours?.LinkageArea ?? Behaviour.Default
            );

        [YamlIgnore]
        [BsonIgnore]
        public ILinkedAccountsAreaBehaviour LinkedAccountsAreaBehaviour =>
            ServiceProvider?.ResolveAreaBehaviour<ILinkedAccountsAreaBehaviour>(
                Behaviours?.LinkedAccounts ?? Behaviour.Default
            );

        [YamlIgnore]
        [BsonIgnore]
        public IPatientRecordAreaBehaviour PatientRecordAreaBehaviour =>
            ServiceProvider?.ResolveAreaBehaviour<IPatientRecordAreaBehaviour>(
                Behaviours?.PatientRecord ?? Behaviour.Default
            );

        [YamlIgnore]
        [BsonIgnore]
        public ICourseAreaBehaviour CourseAreaBehaviour =>
            ServiceProvider?.ResolveAreaBehaviour<ICourseAreaBehaviour>(
                Behaviours?.Course ?? Behaviour.Default
            );

        [YamlIgnore]
        [BsonIgnore]
        public IPrescriptionAreaBehaviour PrescriptionAreaBehaviour =>
            ServiceProvider?.ResolveAreaBehaviour<IPrescriptionAreaBehaviour>(
                Behaviours?.Prescription ?? Behaviour.Default
            );

        [YamlIgnore]
        [BsonIgnore]
        public ISessionAreaBehaviour SessionAreaBehaviour =>
            ServiceProvider?.ResolveAreaBehaviour<ISessionAreaBehaviour>(
                Behaviours?.Session ?? Behaviour.Default
            );

        [YamlIgnore]
        [BsonIgnore]
        public ISessionExtendAreaBehaviour SessionExtendAreaBehaviour =>
            ServiceProvider?.ResolveAreaBehaviour<ISessionExtendAreaBehaviour>(
                Behaviours?.SessionExtend ?? Behaviour.Default
            );
    }
}
