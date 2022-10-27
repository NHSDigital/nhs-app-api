using System.Collections.Generic;

namespace NHSOnline.HttpMocks.SecondaryCare
{
    public static class Constants
    {
        private const string KnownServiceBaseUrlAccurx = "http://accurxwayfinder.stubs.local.bitraft.io:8080";
        private const string KnownServiceBaseUrlDrDoctor = "http://drdoctor.stubs.local.bitraft.io:8080";
        private const string KnownServiceBaseUrlErs = "http://ers.stubs.local.bitraft.io:8080";
        private const string KnownServiceBaseUrlGncr = "http://gncr.stubs.local.bitraft.io:8080";
        private const string KnownServiceBaseUrlHealthcareComms = "http://hcc.stubs.local.bitraft.io:8080";
        private const string KnownServiceBaseUrlNetcall = "http://netcall.stubs.local.bitraft.io:8080";
        private const string KnownServiceBaseUrlPkb = "https://pkb.securestubs.local.bitraft.io";
        private const string KnownServiceBaseUrlZesty = "http://zesty.stubs.local.bitraft.io:8080";
        private const string KnownServiceBaseUrlNbs = "http://nbs.stubs.local.bitraft.io:8080";

        public static Dictionary<ServiceSpecialty, string> ServiceSpecialtyMappings { get; } =
            new()
            {
                {ServiceSpecialty.General, "General"},
                {ServiceSpecialty.Cardiology, "Cardiology"},
                {ServiceSpecialty.Neurology, "Neurology"},
                {ServiceSpecialty.Paediatrics, "Paediatrics"},
                {ServiceSpecialty.ENT, "Ear, Nose and Throat (ENT)"},
                {ServiceSpecialty.Haematology, "Haematology"},
                {ServiceSpecialty.None, string.Empty}
            };

        public static Dictionary<ServiceProvider, string> ProviderUrlMappings { get; } =
            new()
            {
                {ServiceProvider.Accurx, $"{KnownServiceBaseUrlAccurx}/api/OpenIdConnect/AuthenticateManageAppointment?appointmentToken={{0}}"},
                {ServiceProvider.DrDoctor, $"{KnownServiceBaseUrlDrDoctor}/appointments/{{0}}?from=nhsApp"},
                {ServiceProvider.eRS, $"{KnownServiceBaseUrlErs}/nhslogin?ubrn={{0}}"},
                {ServiceProvider.Gncr, $"{KnownServiceBaseUrlGncr}/Appointment/{{0}}"},
                {ServiceProvider.HealthcareComms, $"{KnownServiceBaseUrlHealthcareComms}/appointments/{{0}}"},
                {ServiceProvider.NBS, $"{KnownServiceBaseUrlNbs}/book-a-coronavirus-vaccination/start-page"},
                {ServiceProvider.Netcall, $"{KnownServiceBaseUrlNetcall}/Appointments?id={{0}}&trust=789"},
                {ServiceProvider.PKB, $"{KnownServiceBaseUrlPkb}/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D{{0}}%26nhsStyle%3Dtrue"},
                {ServiceProvider.Zesty, $"{KnownServiceBaseUrlZesty}/nhs/origin_appointment?resource_id={{0}}"},
            };
    }
}