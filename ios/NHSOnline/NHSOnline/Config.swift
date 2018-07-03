import Foundation

struct Config: Decodable {
    private enum CodingKeys: String, CodingKey {
        case HomeUrl, Nhs111Url, Nhs111LocationUrl, OrganDonationUrl, DataSharingUrl
        case AppointmentsUrlPath, MoreUrlPath, PrescriptionsUrlPath, MyAccountUrlPath, SessionUrlPath, MyRecordUrlPath
        case NhsOnlineRequiredQueryString
        case ResponseWaitingTime
    }
    
    let HomeUrl: String
    let Nhs111Url: String
    let Nhs111LocationUrl: String
    let OrganDonationUrl: String
    let DataSharingUrl: String

    let MyRecordUrlPath: String
    let MoreUrlPath: String
    let AppointmentsUrlPath: String
    let PrescriptionsUrlPath: String
    let MyAccountUrlPath: String
    let SessionUrlPath: String

    let NhsOnlineRequiredQueryString:String
    let ResponseWaitingTime: Double
}

func config() -> Config {
    let url = Bundle.main.url(forResource: "Config", withExtension: "plist")!
    let data = try! Data(contentsOf: url)
    let decoder = PropertyListDecoder()
    return try! decoder.decode(Config.self, from: data)
}
