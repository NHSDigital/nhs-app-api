import Foundation

struct Configuration: Codable {
    let isDeviceSupported: Bool
    let isThrottlingEnabled: Bool?
    let fidoServerUrl: String
}
