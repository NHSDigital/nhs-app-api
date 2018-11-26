import Foundation

struct Config: Decodable {
    private enum CodingKeys: String, CodingKey {
        case HomeUrl, Nhs111Url, Nhs111LocationUrl, OrganDonationUrl, ConditionsUrlPath, CheckSymptomsUrlPath, AppScheme, BaseScheme, HotJarLinkUrl, DataPreferencesURL
        case AppointmentsUrlPath, SymptomsUrlPath, MoreUrlPath, PrescriptionsUrlPath, MyAccountUrlPath, SessionUrlPath, MyRecordUrlPath
        case NhsOnlineRequiredQueryString
        case ResponseWaitingTime, SessionTimeout
        case HelpURL, TermsAndConditionsURL, PrivacyPolicyURL, CookiesPolicyURL, OpenSourceLicencesURL, MedicalRecordAbbreviationsURL, AccessibilityStatementURL
        case BaseApiUrl, ConfigurationApiPath
    }
    
    let HomeUrl: String
    let Nhs111Url: String
    let Nhs111LocationUrl: String
    let OrganDonationUrl: String
    let CheckSymptomsUrlPath: String
    let AppScheme: String
    let BaseScheme: String
    let HotJarLinkUrl: String
    let DataPreferencesURL: String
    
    let MyRecordUrlPath: String
    let MoreUrlPath: String
    let ConditionsUrlPath: String
    let AppointmentsUrlPath: String
    let SymptomsUrlPath: String
    let PrescriptionsUrlPath: String
    let MyAccountUrlPath: String
    let SessionUrlPath: String
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
    let BaseApiUrl: String
    let ConfigurationApiPath: String
}

func config() -> Config {
    let url = Bundle.main.url(forResource: "Info", withExtension: "plist")!
    let data = try! Data(contentsOf: url)
    let decoder = PropertyListDecoder()
    return try! decoder.decode(Config.self, from: data)
}
