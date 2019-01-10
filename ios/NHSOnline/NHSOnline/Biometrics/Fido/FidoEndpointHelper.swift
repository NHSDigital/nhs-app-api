import Foundation

struct FidoEndpointHelper {
    let requestRequestEndpoint: String
    let registrationResponseEndpoint: String
    let authenticationRequestEndpoint: String
    let deregistrationRequestEndpoint: String

    init() {
        let fidoServer = ConfigurationService.shared().FidoServerUrl()
        self.requestRequestEndpoint = fidoServer + config().BiometricsRegistrationRequestEndpoint
        self.registrationResponseEndpoint = fidoServer + config().BiometricsRegistrationResponseEndpoint
        self.authenticationRequestEndpoint = fidoServer + config().BiometricsAuthenticationRequestEndpoint
        self.deregistrationRequestEndpoint = fidoServer + config().BiometricsDeregistrationRequestEndpoint

    }
}
