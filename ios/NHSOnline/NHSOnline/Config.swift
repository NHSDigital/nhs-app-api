import Foundation

struct Config: Decodable {
    private enum CodingKeys: String, CodingKey {
        case HomeUrl, Nhs111Url, Nhs111LocationUrl, CheckSymptomsUrlPath, AppScheme, BaseScheme, HotJarLinkUrl, DataPreferencesURL, HomeHost, RedirectorUrl
        case AppointmentsUrlPath, AppontmentsGpAtHandUrlPath, InformaticaUrlPath,  AdminHelpUrlPath, SymptomsUrlPath, MoreUrlPath, PrescriptionsUrlPath, PrescriptionsGpAtHandUrlPath, MyAccountUrlPath, MyRecordGpAtHandUrlPath, SessionUrlPath, MyRecordUrlPath, AuthRedirectPath, OrganDonationUrlPath, DataSharingUrlPath
        case ResponseWaitingTime, SessionTimeout, ApiCallTimeoutSeconds
        case HelpURL, HelpAccountURL, HelpAppointmentsURL, HelpLoginURL, HelpPrescriptionsURL, HelpRecordURL, TermsAndConditionsURL, PrivacyPolicyURL, CookiesPolicyURL, OpenSourceLicencesURL, MedicalRecordAbbreviationsURL, AccessibilityStatementURL, HelpURLOld, TermsAndConditionsURLOld, PrivacyPolicyURLOld, CookiesPolicyURLOld, OpenSourceLicencesURLOld, MedicalRecordAbbreviationsURLOld, AccessibilityStatementURLOld, ConditionsUrlPath, DataSharingUrl
        case BaseApiUrl, ConfigurationApiPath, AppStoreUrl, SessionCookieName, AAID, PrivateKeyLabel
        case BiometricHelpURL, FidoLoginErrorPath, BiometricAuthResponseParam, BiometricsRegistrationResponseEndpoint, BiometricsAuthenticationRequestEndpoint, BiometricsRegistrationRequestEndpoint, BiometricsDeregistrationRequestEndpoint, BiometricsAssertionScheme, CidUrlSuffix
        case MenuTimeoutSeconds, LinkPropertyName, DeepLinkAppClosed
    }
    
    let HomeUrl: String
    let Nhs111Url: String
    let Nhs111LocationUrl: String
    let CheckSymptomsUrlPath: String
    let AppScheme: String
    let BaseScheme: String
    let HotJarLinkUrl: String
    let DataPreferencesURL: String
    let RedirectorUrl: String

    let MyRecordUrlPath: String
    let MyRecordGpAtHandUrlPath: String
    let MoreUrlPath: String
    let DataSharingUrlPath: String
    let AppointmentsUrlPath: String
    let AppontmentsGpAtHandUrlPath: String
    let InformaticaUrlPath: String
    let AdminHelpUrlPath: String
    let SymptomsUrlPath: String
    let OrganDonationUrlPath: String
    let PrescriptionsUrlPath: String
    let PrescriptionsGpAtHandUrlPath: String
    let MyAccountUrlPath: String
    let SessionUrlPath: String
    let AuthRedirectPath: String
    let ResponseWaitingTime: Double
    let SessionTimeout: Int

    let HelpURL: String
    let HelpAccountURL: String
    let HelpAppointmentsURL: String
    let HelpLoginURL: String
    let HelpPrescriptionsURL: String
    let HelpRecordURL: String
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
    let ConditionsUrlPath: String
    let DataSharingUrl: String
    
    let BaseApiUrl: String
    let ConfigurationApiPath: String
    let AppStoreUrl: String
    let SessionCookieName: String
    let AAID: String
    let PrivateKeyLabel: String
    let FidoLoginErrorPath: String
    let BiometricHelpURL: String
    let BiometricAuthResponseParam: String
    let BiometricsRegistrationResponseEndpoint: String
    let BiometricsAuthenticationRequestEndpoint: String
    let BiometricsRegistrationRequestEndpoint: String
    let BiometricsDeregistrationRequestEndpoint: String
    let BiometricsAssertionScheme: String
    let CidUrlSuffix: String
    let MenuTimeoutSeconds: Double
    let HomeHost: String
    let ApiCallTimeoutSeconds: Int
    let LinkPropertyName: String
    let DeepLinkAppClosed: String
}

private var instance: Config! = nil

func config() -> Config {
    if (instance != nil) {
        return instance!
    }
    
    let url = Bundle.main.url(forResource: "Info", withExtension: "plist")!
    let data = try! Data(contentsOf: url)
    let decoder = PropertyListDecoder()
    instance = try! decoder.decode(Config.self, from: data)
    
    return instance
}
