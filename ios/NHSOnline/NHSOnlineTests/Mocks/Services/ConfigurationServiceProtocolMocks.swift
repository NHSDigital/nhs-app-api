@testable import NHSOnline

class SuccessConfigurationProtocolMock: ConfigurationServiceProtocol  {
    let configResponse: ConfigurationResponse
    init(configurationResponse: ConfigurationResponse) {
        configResponse = configurationResponse
    }
    func getConfigurationResponse() -> Result<ConfigurationResponse, ConfigurationError> {
        Result.success(configResponse)
    }
}

class SuccessConfigurationResponseMock {
    var instance: ConfigurationResponse
    init() {
        instance = ConfigurationResponse()
        instance.isSupportedVersion = true
        instance.fidoServerUrl = "fidoUrlServer"
        instance.knownServices = CompleteKnownServicesMock()
        instance.callSuccessful = true
    }
}

class SuccessfulInvalidAppVersionConfigurationResponseMock {
    var instance: ConfigurationResponse
    init() {
        instance = ConfigurationResponse()
        instance.isSupportedVersion = false
        instance.fidoServerUrl = "fidoUrlServer"
        instance.knownServices = CompleteKnownServicesMock()
        instance.callSuccessful = true
    }
}

class CompleteKnownServicesMock: KnownServices {
    convenience init(url: String = "http://example.com",
                     javaScriptInteractionMode: JavaScriptInteractionMode = JavaScriptInteractionMode.NhsApp) {
        self.init([RootService(
                url: url,
                javaScriptInteractionMode: javaScriptInteractionMode,
                menuTab: MenuTab.None,
                integrationLevel: .Bronze,
                validateSession: false,
                subServices: [])])
    }
}
