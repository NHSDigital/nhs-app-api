import Foundation
import os.log

class ConfigurationServiceManager{
    
    private static var sharedConfigurationServiceManager = ConfigurationServiceManager()
    private var configurationService: ConfigurationServiceProtocol?
    private var configurationResponse = ConfigurationResponse()

    init() {
        self.configurationService = ConfigurationService.shared()
        ProcessConfigurationResponse()
    }
    
    private func ProcessConfigurationResponse() {
        if !self.configurationResponse.callSuccessful {
            let semaphore = DispatchSemaphore(value: 0)
            configurationService?.getConfigurationResponse() { (configResponse) in
                if let config = configResponse {
                    let validVersion = self.isSupportedVersion(minimumSupportediOSVersion: config.minimumSupportediOSVersion);
                    self.configurationResponse = ConfigurationResponse(
                        true, config.fidoServerUrl, validVersion, KnownServices(config.knownServices)
                    )
                }
                semaphore.signal()
            }
            
            if(semaphore.wait(timeout: DispatchTime.now() + DispatchTimeInterval.seconds(config().ApiCallTimeoutSeconds)) == DispatchTimeoutResult.timedOut)
            {
                Logger.logError(message: "Failure doing native app version http check: %@", "Timed out")
            }
        }
    }

    private func isSupportedVersion(minimumSupportediOSVersion: String) -> Bool {
        let versionNumber = Bundle.main.object(forInfoDictionaryKey: "CFBundleShortVersionString") as! String
        let diff = minimumSupportediOSVersion.compare(versionNumber, options: .numeric)
        return diff != .orderedDescending
    }
    
    func GetConfigurationResponse() -> ConfigurationResponse {
        self.configurationResponse
    }

    public func FidoServerUrl() -> String {
        return configurationResponse.fidoServerUrl
    }
    
    class func shared() -> ConfigurationServiceManager {
         return sharedConfigurationServiceManager
     }
}
