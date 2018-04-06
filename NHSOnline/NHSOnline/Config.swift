import Foundation

struct Config: Decodable {
    private enum CodingKeys: String, CodingKey {
        case BaseUrl, Nhs111Url, OrganDonationUrl,NhsOnlineRequiredQueryString, ResponseWaitingTime
    }
    
    let BaseUrl: String
    let Nhs111Url: String
    let OrganDonationUrl: String
    let NhsOnlineRequiredQueryString:String
    let ResponseWaitingTime: Double
}

func config() -> Config {
    let url = Bundle.main.url(forResource: "Config", withExtension: "plist")!
    let data = try! Data(contentsOf: url)
    let decoder = PropertyListDecoder()
    return try! decoder.decode(Config.self, from: data)
}
