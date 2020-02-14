protocol ConfigurationServiceProtocol {
    func getConfigurationResponse(completion: @escaping(Configuration?) -> ())
}