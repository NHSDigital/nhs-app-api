import Foundation

class FidoEndpointHelper {
    private let configurationServiceProvider: ConfigurationServiceProtocol
    private let config: Config
    private var configLoaded = false
    public var requestRequestEndpoint: String?
    public var registrationResponseEndpoint: String?
    public var authenticationRequestEndpoint: String?
    public var deregistrationRequestEndpoint: String?
    
    public init(configurationServiceProvider: ConfigurationServiceProtocol,
                config: Config) {
        self.configurationServiceProvider = configurationServiceProvider
        self.config = config
    }

    func configure() throws {
        if configLoaded {
            return
        }
        switch configurationServiceProvider.getConfigurationResponse() {
        case .success(let value):
            self.requestRequestEndpoint = value.fidoServerUrl + config.BiometricsRegistrationRequestEndpoint
            self.registrationResponseEndpoint = value.fidoServerUrl + config.BiometricsRegistrationResponseEndpoint
            self.authenticationRequestEndpoint = value.fidoServerUrl + config.BiometricsAuthenticationRequestEndpoint
            self.deregistrationRequestEndpoint = value.fidoServerUrl + config.BiometricsDeregistrationRequestEndpoint
            configLoaded = true
        default:
            Logger.logError(message: "Loading of config failed, using default values for fido endpoints")
            throw ConfigurationError.configurationLoadFailed
        }
    }
}
