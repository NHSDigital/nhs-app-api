import Foundation

struct Configuration: Codable {
    let fidoServerUrl: String
    let minimumSupportediOSVersion: String
    let knownServices: [RootService]

    init(fidoServerUrl: String, minimumSupportediOSVersion: String, knownServices: [RootService]) {
        self.fidoServerUrl = fidoServerUrl
        self.minimumSupportediOSVersion = minimumSupportediOSVersion
        self.knownServices = knownServices
    }
}
