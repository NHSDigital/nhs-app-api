@testable import NHSOnline

class MockConfigurationService {
    var isSupportedVersion = false
    var mockRootServices: [RootService] = []

    func isUserDeviceAllowed(homeViewController: HomeViewController, completionHandler: @escaping (ConfigurationResponse?) -> Void) {

        let response = ConfigurationResponse(false, "", isSupportedVersion)

        completionHandler(response)
    }

    func getConfigurationResponse(completion: @escaping (Configuration?) -> ()) {
        mockRootServices.append(RootService.init(url: "https://test.com", javaScriptInteractionMode: .NhsApp,
                menuTab: .Advice,  integrationLevel: .Bronze, validateSession: false, subServices: nil))

        let response = Configuration(fidoServerUrl: "", minimumSupportediOSVersion: "", knownServices: mockRootServices, nhsLoginLoggedInPaths: [String]())
        completion(response)
    }
}
