import Foundation

struct Config: Decodable {
    private enum CodingKeys: String, CodingKey {
        case HomeUrl, Nhs111Url, Nhs111LocationUrl, OrganDonationUrl, DataSharingUrl, ConditionsUrlPath, CheckSymptomsUrlPath, AppScheme, BaseScheme, HotJarLinkUrl, DataPreferencesURL
        case AppointmentsUrlPath, SymptomsUrlPath, MoreUrlPath, PrescriptionsUrlPath, MyAccountUrlPath, SessionUrlPath, MyRecordUrlPath
        case NhsOnlineRequiredQueryString
        case ResponseWaitingTime
        case IsFirstTimeOpened, CarouselDirectory, CarouselContentType, CarouselFileName
        case HelpURL, TermsAndConditionsURL, PrivacyPolicyURL, CookiesPolicyURL, OpenSourceLicensesURL, MedicalRecordAbbreviationsURL
    }
    
    let HomeUrl: String
    let Nhs111Url: String
    let Nhs111LocationUrl: String
    let OrganDonationUrl: String
    let DataSharingUrl: String
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
    
    let IsFirstTimeOpened: String
    let CarouselDirectory: String
    let CarouselContentType: String
    let CarouselFileName: String
    
    let HelpURL: String
    let TermsAndConditionsURL: String
    let PrivacyPolicyURL: String
    let CookiesPolicyURL: String
    let OpenSourceLicensesURL: String
    let MedicalRecordAbbreviationsURL: String
}

func config() -> Config {
    let url = Bundle.main.url(forResource: "Config", withExtension: "plist")!
    let data = try! Data(contentsOf: url)
    let decoder = PropertyListDecoder()
    return try! decoder.decode(Config.self, from: data)
}
