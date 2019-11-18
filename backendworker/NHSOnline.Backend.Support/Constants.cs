using System;
using System.Collections.Generic;

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
            public const int Status550SupplierDoesNotSupportLinkageKeys = 550;
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
            public const string PatientId = NhsoPrefix + "Patient-Id";
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
            public const string ClientSideLogMessageWhitelist = @"[^a-zA-Z0-9().;\-:/ \r\n]";
            public const string ClientSideLogMessageNewLine = "\r\n";
        }

        public static class FileConstants
        {
            public static class FileTypes
            {
                public static class ImageType
                {
                    public const string Bmp = "bmp";
                    public const string Dib = "dib";
                    public const string Gif = "gif";
                    public const string Jpg = "jpg";
                    public const string Jpeg = "jpeg";
                    public const string Jpe = "jpe";
                    public const string Jfif = "jfif";
                    public const string Tif = "tif";
                    public const string Tiff = "tiff";
                    public const string Png = "png";
                    public const string Tga = "tga";
                    public const string Tpic = "tpic";
                }
        
                public static readonly List<string> ImageTypes = new List<string>
                {
                    ImageType.Bmp,
                    ImageType.Dib,
                    ImageType.Gif,
                    ImageType.Jpg,
                    ImageType.Jpeg,
                    ImageType.Jpe,
                    ImageType.Jfif,
                    ImageType.Tif,
                    ImageType.Tiff,
                    ImageType.Png,
                    ImageType.Tga,
                    ImageType.Tpic
                };
        
                public static class DocumentType
                {
                    public const string Pdf = "pdf";
                    public const string Doc = "doc";
                    public const string Docx = "docx";
                    public const string Docm = "docm";
                    public const string Dot = "dot";
                    public const string Rtf = "rtf";
                }
        
                public static readonly List<string> DocumentTypes = new List<string>
                {
                    DocumentType.Doc,
                    DocumentType.Docx,
                    DocumentType.Docm,
                    DocumentType.Dot
                };
                
                public static class TextType
                {
                    public const string Txt = "txt";
                    public const string Rtf = "rtf";
                }
        
                public static readonly List<string> TextTypes = new List<string>
                {
                    TextType.Txt,
                    TextType.Rtf
                };
                
                public static readonly Dictionary<string, string> DocumentMimeTypes = new Dictionary<string, string>
                {
                    { DocumentType.Pdf, "application/pdf" },
                    { ImageType.Bmp, "image/bmp" },
                    { ImageType.Dib, "image/dib" },
                    { ImageType.Gif, "image/gif" },
                    { ImageType.Jpg, "image/jpg" },
                    { ImageType.Jpeg, "image/jpg" },
                    { ImageType.Jpe, "image/jpg" },
                    { ImageType.Jfif, "image/jpg" },
                    { ImageType.Tif, "image/tiff" },
                    { ImageType.Tiff, "image/tiff" },
                    { ImageType.Png, "image/png" },
                    { ImageType.Tga, "image/x-tga" },
                    { ImageType.Tpic, "image/x-tga" },
                    { DocumentType.Doc, "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                    { DocumentType.Docx, "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                    { DocumentType.Docm, "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                    { DocumentType.Dot, "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                    { TextType.Rtf, "application/rtf"},
                    { TextType.Txt, "text/plain"}
                };
            }
        }

        public static class TppConstants
        {
            public const string RequestIdentifierHeader = "type";
        }

        public static class OnlineConsultationConstants
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

        public static class UsersConstants
        {
            public const string TagSeparator = ":";
            public const string NhsLoginIdTagPrefix = "nhsLoginId";
        }
    }
}
