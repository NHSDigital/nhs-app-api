import Foundation

struct Config: Decodable {
    private enum CodingKeys: String, CodingKey {
        case HomeUrl, Nhs111Url, OrganDonationUrl
        case AppointmentsUrlPath, MoreUrlPath, PrescriptionsUrlPath
        case TitleNHS111, TitleOrganDonation
        case NhsOnlineRequiredQueryString
        case ResponseWaitingTime
        case ErrorMessageGeneric, ErrorMessageOrganDonation, ErrorMessageNhs111
    }
    
    let HomeUrl: String
    let Nhs111Url: String
    let OrganDonationUrl: String
    
    let MoreUrlPath: String
    let AppointmentsUrlPath: String
    let PrescriptionsUrlPath: String

    let TitleNHS111:String
    let TitleOrganDonation:String

    let NhsOnlineRequiredQueryString:String
    let ResponseWaitingTime: Double
    
    let ErrorMessageGeneric: String
    let ErrorMessageOrganDonation: String
    let ErrorMessageNhs111: String
}

func config() -> Config {
    let url = Bundle.main.url(forResource: "Config", withExtension: "plist")!
    let data = try! Data(contentsOf: url)
    let decoder = PropertyListDecoder()
    return try! decoder.decode(Config.self, from: data)
}
