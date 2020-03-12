protocol KnownServicesProtocol {
    func getKnownServices() -> Result<KnownServices, ConfigurationError>
}
