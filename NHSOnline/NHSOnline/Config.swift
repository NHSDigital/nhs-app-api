import Foundation

struct Config: Decodable {
    private enum CodingKeys: String, CodingKey {
        case HomeUrl, Nhs111Url, OrganDonationUrl
        case AppointmentsUrlPath, MoreUrlPath
        case TitleNHS111, TitleOrganDonation
        case NhsOnlineRequiredQueryString
        case ResponseWaitingTime
    }
    
    let HomeUrl: String
    let Nhs111Url: String
    let OrganDonationUrl: String
    
    let MoreUrlPath: String
    let AppointmentsUrlPath: String
    
    let TitleNHS111:String
    let TitleOrganDonation:String
    
    let NhsOnlineRequiredQueryString:String
    let ResponseWaitingTime: Double
}

func config() -> Config {
    let url = Bundle.main.url(forResource: "Config", withExtension: "plist")!
    let data = try! Data(contentsOf: url)
    let decoder = PropertyListDecoder()
    return try! decoder.decode(Config.self, from: data)
}
