import Foundation

struct Configuration: Codable {
    let fidoServerUrl: String
    let minimumSupportediOSVersion: String
    let knownServices: [RootService]
    let nhsLoginLoggedInPaths: [String]

    init(fidoServerUrl: String, minimumSupportediOSVersion: String, knownServices: [RootService], nhsLoginLoggedInPaths: [String]) {
        self.fidoServerUrl = fidoServerUrl
        self.minimumSupportediOSVersion = minimumSupportediOSVersion
        self.knownServices = knownServices
        self.nhsLoginLoggedInPaths = nhsLoginLoggedInPaths
    }
}
