struct ConfigurationResponse {
    var callSuccessful: Bool
    var fidoServerUrl: String
    var isSupportedVersion: Bool
    var knownServices: KnownServices

    init(_ callSuccessful: Bool = false,
         _ fidoServerUrl: String = "",
         _ isSupportedVersion: Bool = false,
         _ knownServices: KnownServices = KnownServices()){
        self.callSuccessful = callSuccessful
        self.fidoServerUrl = fidoServerUrl
        self.isSupportedVersion = isSupportedVersion
        self.knownServices = knownServices
    }
}
