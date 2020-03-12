protocol ConfigurationServiceProtocol {
    func getConfigurationResponse() -> Result<ConfigurationResponse, ConfigurationError>
}

enum ConfigurationError: Error {
    case configurationLoadFailed
}
