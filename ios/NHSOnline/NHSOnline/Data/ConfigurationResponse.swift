struct ConfigurationResponse {
    
    init(_ isValidConfiguration: Bool = false, _ isThrottlingEnabled: Bool = false, _ FidoServerUrl: String = "", _ CallFailed: Bool = true) {
        self.isValidConfiguration = isValidConfiguration
        self.isThrottlingEnabled = isThrottlingEnabled
        self.FidoServerUrl = FidoServerUrl
        self.callFailed = CallFailed
    }
    
    public var isValidConfiguration: Bool
    public var isThrottlingEnabled: Bool
    public var FidoServerUrl: String
    public var callFailed: Bool
}
