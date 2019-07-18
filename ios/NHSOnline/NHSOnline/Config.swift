import Foundation

struct Config: Decodable {
    private enum CodingKeys: String, CodingKey {
        case HomeUrl, Nhs111Url, Nhs111LocationUrl, ConditionsUrlPath, CheckSymptomsUrlPath, AppScheme, BaseScheme, HotJarLinkUrl, DataPreferencesURL, HomeHost
        case AppointmentsUrlPath, AdminHelpUrlPath, SymptomsUrlPath, MoreUrlPath, PrescriptionsUrlPath, MyAccountUrlPath, SessionUrlPath, MyRecordUrlPath, AuthRedirectPath, OrganDonationUrlPath, DataSharingUrlPath
        case NhsOnlineRequiredQueryString
        case ResponseWaitingTime, SessionTimeout, ApiCallTimeoutSeconds
        case HelpURL, TermsAndConditionsURL, PrivacyPolicyURL, CookiesPolicyURL, OpenSourceLicencesURL, MedicalRecordAbbreviationsURL, AccessibilityStatementURL, HelpURLOld, TermsAndConditionsURLOld, PrivacyPolicyURLOld, CookiesPolicyURLOld, OpenSourceLicencesURLOld, MedicalRecordAbbreviationsURLOld, AccessibilityStatementURLOld
        case BaseApiUrl, ConfigurationApiPath, AppStoreUrl, SessionCookieName, AAID, PrivateKeyLabel
        case BiometricHelpURL, FidoLoginErrorPath, BiometricRedirectURL, BiometricsRegistrationResponseEndpoint, BiometricsAuthenticationRequestEndpoint, BiometricsRegistrationRequestEndpoint, BiometricsDeregistrationRequestEndpoint, BiometricsAssertionScheme, CidUrlSuffix
        case MenuTimeoutSeconds
    }
    
    let HomeUrl: String
    let Nhs111Url: String
    let Nhs111LocationUrl: String
    let CheckSymptomsUrlPath: String
    let AppScheme: String
    let BaseScheme: String
    let HotJarLinkUrl: String
    let DataPreferencesURL: String

    let MyRecordUrlPath: String
    let MoreUrlPath: String
    let ConditionsUrlPath: String
    let AppointmentsUrlPath: String
    let AdminHelpUrlPath: String
    let SymptomsUrlPath: String
    let OrganDonationUrlPath: String
    let PrescriptionsUrlPath: String
    let MyAccountUrlPath: String
    let DataSharingUrlPath: String
    let SessionUrlPath: String
    let AuthRedirectPath: String
    let NhsOnlineRequiredQueryString:String
    let ResponseWaitingTime: Double
    let SessionTimeout: Int

    let HelpURL: String
    let TermsAndConditionsURL: String
    let PrivacyPolicyURL: String
    let CookiesPolicyURL: String
    let OpenSourceLicencesURL: String
    let MedicalRecordAbbreviationsURL: String
    let AccessibilityStatementURL: String
    let HelpURLOld: String
    let TermsAndConditionsURLOld: String
    let PrivacyPolicyURLOld: String
    let CookiesPolicyURLOld: String
    let OpenSourceLicencesURLOld: String
    let MedicalRecordAbbreviationsURLOld: String
    let AccessibilityStatementURLOld: String
    
    let BaseApiUrl: String
    let ConfigurationApiPath: String
    let AppStoreUrl: String
    let SessionCookieName: String
    let AAID: String
    let PrivateKeyLabel: String
    let FidoLoginErrorPath: String
    let BiometricHelpURL: String
    let BiometricRedirectURL: String
    let BiometricsRegistrationResponseEndpoint: String
    let BiometricsAuthenticationRequestEndpoint: String
    let BiometricsRegistrationRequestEndpoint: String
    let BiometricsDeregistrationRequestEndpoint: String
    let BiometricsAssertionScheme: String
    let CidUrlSuffix: String
    let MenuTimeoutSeconds: Double
    let HomeHost: String
    let ApiCallTimeoutSeconds: Int
}

func config() -> Config {
    let url = Bundle.main.url(forResource: "Info", withExtension: "plist")!
    let data = try! Data(contentsOf: url)
    let decoder = PropertyListDecoder()
    return try! decoder.decode(Config.self, from: data)
}
