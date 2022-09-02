using System.Collections.Generic;

namespace NHSOnline.HttpMocks.SecondaryCare
{
    public static class Constants
    {
        // These mocks support deep links for environments which dont have Known Services overrides.
        // This includes local and Sandpit.
        private const string KnownServiceBaseUrlAccurx = "http://accurxwayfinder.stubs.local.bitraft.io:8080";
        private const string KnownServiceBaseUrlDrDoctor = "http://drdoctor.stubs.local.bitraft.io:8080";
        private const string KnownServiceBaseUrlErs = "http://ers.stubs.local.bitraft.io:8080";
        private const string KnownServiceBaseUrlNetcall = "http://netcall.stubs.local.bitraft.io:8080";
        private const string KnownServiceBaseUrlPkb = "https://pkb.securestubs.local.bitraft.io";
        private const string KnownServiceBaseUrlZesty = "http://zesty.stubs.local.bitraft.io:8080";

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
                {ServiceProvider.PKB, $"{KnownServiceBaseUrlPkb}/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D{{0}}%26nhsStyle%3Dtrue"},
                {ServiceProvider.eRS, $"{KnownServiceBaseUrlErs}/nhslogin?ubrn={{0}}"},
                {ServiceProvider.DrDoctor, $"{KnownServiceBaseUrlDrDoctor}/appointments/{{0}}?from=nhsApp"},
                {ServiceProvider.Netcall, $"{KnownServiceBaseUrlNetcall}/i/nhsappintegration/p/27EFBAC1/40EFBAC1/66EFBAC1/417EFBAC1?remote_record_id={{0}}"},
                {ServiceProvider.Zesty, $"{KnownServiceBaseUrlZesty}/nhs/origin_appointment?resource_id={{0}}"},
            };
    }
}