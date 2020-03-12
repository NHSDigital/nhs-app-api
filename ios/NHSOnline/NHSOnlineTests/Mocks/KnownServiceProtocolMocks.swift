@testable import NHSOnline

class SuccessKnownServiceProtocolMock: KnownServicesProtocol {
    let knownServiceMock: KnownServices
    init(knownServiceMock: KnownServices = CompleteKnownServicesMock())
    {
        self.knownServiceMock = knownServiceMock
    }

    func getKnownServices() -> Result<KnownServices, ConfigurationError> {
        Result.success(knownServiceMock)
    }
}
