namespace NHSOnline.Backend.Support
{
    public static class Constants
    {
        public static class ClaimTypes
        {
            public const string SessionId = "SessionId";
        }

        public static class CookieNames
        {
            public const string SessionId = "NHSO-Session-Id";
        }

        public static class CustomHttpStatusCodes
        {
            public const int Status460LimitReached = 460;
            public const int Status461TooLate = 461;
            public const int Status462FailedToRecordConsent = 462;
            public const int Status463FailedToFetchConsent = 463;
            public const int Status464OdsCodeNotSupportedOrNoNhsNumber = 464;
            public const int Status465FailedAgeRequirement = 465;
            public const int Status466MedicationAlreadyOrderedWithinLast30Days = 466;
        }

        public static class AppConfig
        {
            public const string GitCommitId = "Version:CommitId";
            public const string ThrottlingEnabled = "ThrottlingEnabled";
        }

        public static class EnvironmentalVariables
        {
            public const string VersionTag = "VERSION_TAG";
        }

        public static class HttpHeaders
        {
            private const string NhsoPrefix = "NHSO-";
            public const string ConnectionToken = NhsoPrefix + "Connection-Token";
            public const string OdsCode = NhsoPrefix + "ODS-Code";
            public const string NhsNumber = NhsoPrefix + "Nhs-Number";
            public const string Surname = NhsoPrefix + "Surname";
            public const string DateOfBirth = NhsoPrefix + "Date-Of-Birth";
            public const string IdentityToken = NhsoPrefix + "Identity-Token";
            public const string WebAppVersion = NhsoPrefix + "Web-Version-Tag";
            public const string NativeAppVersion = NhsoPrefix + "Native-Version-Tag";
            public const string CorrelationIdentifier = NhsoPrefix + "Request-ID";
            public const string LoginClient = NhsoPrefix + "NHS-Login-Client";
            public const string JavascriptDisabled = NhsoPrefix + "Javascript-Disabled";
        }

        public static class HttpContextItems
        {
            public const string UserSession = "UserSession";
        }

        public static class OdsCodeFormats
        {
            public const string GpPracticeEnglandWales = @"^[A-Z0-9]{6}$";
        }

        public static class Regex
        {
            public const string GuidRegex = @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}";
        }

        public static class TppConstants
        {
            public const string RequestIdentifierHeader = "type";
        }
        
        public static class VisionConstants
        {
            public const string RequestIdentifierHeader = "type";
        }

        public static class SupportedDeviceNames
        {
            public const string Android = "Android";
            public const string iOS = "iOS";
        }

        public static class OrganDonationConstants
        {
            public const string SessionIdHeaderKey = "X-Session-ID";
            public const string SequenceIdHeaderKey = "X-Sequence-ID";
            public const string AllOrgansChoiceKey = "all";
            public const string YesChoiceValue = "yes";
            public const string NoChoiceValue = "no";
            public const string NotStatedChoiceValue = "not-stated";
            public const string DateFormat = "yyyy-MM-dd";
            public const string IdentifierSystem = "https://fhir.nhs.uk/Id/nhs-number";
            public const string ReligiousCodingSystem = "http://www.nhsbt.nhs.uk/fhir/religious-affiliations";
            public const string EthnicityCodingSystem = "http://www.nhsbt.nhs.uk/fhir/ethnic-categories";
            public const string WithdrawReasonCodingSystem = "http://www.nhsbt.nhs.uk/fhir/withdraw-reasons";
        }

        public static class OnlineConsultationsConstants
        {
           public const string DemographicsOptionCode = "GLO_PRE_DISCLAIMERS_DEMOGRAPHIC"; 
           public const string DemographicsLabel = "I consent to the NHS App sharing my name, date of birth, gender, NHS number and postal address with {0}. (Optional)";
        }
    }
}
