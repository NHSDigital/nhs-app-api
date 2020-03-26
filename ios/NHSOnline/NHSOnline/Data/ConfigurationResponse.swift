struct ConfigurationResponse {
    var callSuccessful: Bool
    var fidoServerUrl: String
    var isSupportedVersion: Bool
    var nhsLoginLoggedInPaths: [String]
    var knownServices: KnownServices

    init(_ callSuccessful: Bool = false,
         _ fidoServerUrl: String = "",
         _ isSupportedVersion: Bool = false,
         _ nhsLoginLoggedInPaths: [String] = [String](),
         _ knownServices: KnownServices = KnownServices()){
        self.callSuccessful = callSuccessful
        self.fidoServerUrl = fidoServerUrl
        self.isSupportedVersion = isSupportedVersion
        self.nhsLoginLoggedInPaths = nhsLoginLoggedInPaths
        self.knownServices = knownServices
    }
}
