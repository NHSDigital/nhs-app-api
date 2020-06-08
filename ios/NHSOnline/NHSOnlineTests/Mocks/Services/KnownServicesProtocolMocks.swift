@testable import NHSOnline

class KnownServicesProtocolMocks: KnownServicesProtocol {
    var knownServicesMock: KnownServices?
    
    init(knownServicesMock: KnownServices? = nil)
    {
        self.knownServicesMock = knownServicesMock
    }

    func getKnownServices() -> Result<KnownServices, ConfigurationError> {
        return knownServicesMock != nil
            ? Result.success(knownServicesMock!)
            : Result.failure(.configurationLoadFailed)
    }
    
    static func success(knownServicesMock: KnownServices = CompleteKnownServicesMock()) ->  KnownServicesProtocolMocks {
        return KnownServicesProtocolMocks(knownServicesMock: knownServicesMock)
    }
}
