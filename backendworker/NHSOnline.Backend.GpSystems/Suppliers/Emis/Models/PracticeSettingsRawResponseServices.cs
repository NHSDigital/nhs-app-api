using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Session;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class PracticeSettingsRawResponseServices
    {
        public bool AppointmentsSupported { get; set; }
        public bool PrescribingSupported { get; set; }
        public bool EpsSupported { get; set; }
        public bool DemographicsUpdateSupported { get; set; }
        public bool PracticePatientCommunicationSupported { get; set; }
        public bool OnlineRegistrationSupported { get; set; }
        public bool PreRegistrationSupported { get; set; }
        public bool OnlineTriageSupported { get; set; }
        public bool MedicalRecordSupported { get; set; }
        public bool RecordSharingSupported { get; set; }

        public void PrintIsAppointmentsSupportedSupportedEnabled(
            ILogger<EmisSessionService> logger, string odsCode)
        {
            PrintEnabledLog(logger,odsCode,AppointmentsSupported, "Appointments");
        }
        
        public void PrintIsPrescribingSupportedEnabled(
            ILogger<EmisSessionService> logger, string odsCode)
        {
            PrintEnabledLog(logger,odsCode,PrescribingSupported, "Prescriptions");
        }
        
        public void PrintIsDemographicsUpdateSupportedEnabled(
            ILogger<EmisSessionService> logger, string odsCode)
        {
            PrintEnabledLog(logger,odsCode,DemographicsUpdateSupported, "Demographics");
        }
        
        public void PrintIsOnlineRegistrationSupportedEnabled(
            ILogger<EmisSessionService> logger, string odsCode)
        {
            PrintEnabledLog(logger,odsCode,OnlineRegistrationSupported, "Online Registration");
        }
        
        public void PrintIsEpsSupportedEnabled(
            ILogger<EmisSessionService> logger, string odsCode)
        {
            PrintEnabledLog(logger,odsCode,EpsSupported, "EPS");
        }
        
        public void PrintIsPreRegistrationSupportedEnabled(
            ILogger<EmisSessionService> logger, string odsCode)
        {
            PrintEnabledLog(logger,odsCode,PreRegistrationSupported, "Pre Registration");
        }
        
        public void PrintIsPracticePatientCommunicationSupportedEnabled(
            ILogger<EmisSessionService> logger, string odsCode)
        {
            PrintEnabledLog(logger,odsCode,PracticePatientCommunicationSupported, "PFS messaging");
        }
        
        public void PrintIsOnlineTriageSupportedEnabled(
            ILogger<EmisSessionService> logger, string odsCode)
        {
            PrintEnabledLog(logger,odsCode,OnlineTriageSupported, "Online Triage");
        }
        
        public void PrintIsRecordSharingSupportedEnabled(
            ILogger<EmisSessionService> logger, string odsCode)
        {
            PrintEnabledLog(logger,odsCode,RecordSharingSupported, "Record Sharing");
        }
        
        public void PrintIsMedicalRecordSupportedEnabled(
            ILogger<EmisSessionService> logger, string odsCode)
        {
            PrintEnabledLog(logger,odsCode,MedicalRecordSupported, "Medical Record");
        }

        private static void PrintEnabledLog(ILogger logger, string odsCode, bool enabled, string service)
        {
            logger.LogInformation(
                $"ODSCode {odsCode} {service} enabled: {enabled}");
        }
    }
    
    
}