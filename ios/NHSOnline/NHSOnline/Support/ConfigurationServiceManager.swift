import Foundation
import os.log

class ConfigurationServiceManager: ConfigurationServiceProtocol, KnownServicesProtocol {
    private static var sharedConfigurationServiceManager = ConfigurationServiceManager()
    private var configurationResponse = ConfigurationResponse()

    private init() {
    }

    private func ProcessConfigurationResponse() {
        let semaphore = DispatchSemaphore(value: 0)
        if self.configurationResponse.callSuccessful {
            semaphore.signal()
            return
        }

        let configurationService = ConfigurationService()
        configurationService.getConfigurationResponse() { (configResponse) in
            defer { semaphore.signal() }
            if let config = configResponse {
                let validVersion = self.isSupportedVersion(minimumSupportediOSVersion: config.minimumSupportediOSVersion);
                self.configurationResponse = ConfigurationResponse(
                        true, config.fidoServerUrl, validVersion, KnownServices(config.knownServices)
                )
            }
        }

        if (semaphore.wait(timeout: DispatchTime.now() + DispatchTimeInterval.seconds(config().ApiCallTimeoutSeconds)) == DispatchTimeoutResult.timedOut) {
            Logger.logError(message: "Failure doing native app version http check: %@", "Timed out")
        }
    }

    private func isSupportedVersion(minimumSupportediOSVersion: String) -> Bool {
        let versionNumber = Bundle.main.object(forInfoDictionaryKey: "CFBundleShortVersionString") as! String
        let diff = minimumSupportediOSVersion.compare(versionNumber, options: .numeric)
        return diff != .orderedDescending
    }

    func getConfigurationResponse() -> Result<ConfigurationResponse, ConfigurationError> {
        self.ProcessConfigurationResponse()
        if (self.configurationResponse.callSuccessful) {
            return Result.success(self.configurationResponse)
        }

        return Result.failure(.configurationLoadFailed)
    }

    func getKnownServices() -> Result<KnownServices, ConfigurationError> {
        return self.getConfigurationResponse().map({ $0.knownServices })
    }

    static func getConfigurationServiceProvider() -> ConfigurationServiceProtocol {
        return sharedConfigurationServiceManager
    }

    static func getKnownServiceProvider() -> KnownServicesProtocol {
        return sharedConfigurationServiceManager
    }
}
