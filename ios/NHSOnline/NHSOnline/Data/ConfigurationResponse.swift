struct ConfigurationResponse {
    
    init(_ isValidConfiguration: Bool = false, _ FidoServerUrl: String = "", _ CallFailed: Bool = true) {
        self.isValidConfiguration = isValidConfiguration
        self.FidoServerUrl = FidoServerUrl
        self.callFailed = CallFailed
    }
    
    public var isValidConfiguration: Bool
    public var FidoServerUrl: String
    public var callFailed: Bool
}
