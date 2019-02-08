import Foundation
import os.log
protocol ConfigurationServiceProtocol {
    func isUserDeviceAllowed(homeViewController: HomeViewController, completionHandler: @escaping (ConfigurationResponse?) -> Void)
}
class   ConfigurationService : ConfigurationServiceProtocol {
    
    private static var sharedConfigurationService = ConfigurationService()
    private var configurationResponse = ConfigurationResponse()
    
    private init() {
        doConfigurationCallAndValidateResponse()
    }
    
    private func makeConfigCall() {
        if self.configurationResponse.callFailed {
            let semaphore = DispatchSemaphore(value: 0)
            getConfigurationResponse() { (configResponse) in
                if let config = configResponse {
                    self.configurationResponse = ConfigurationResponse(config.isDeviceSupported,
                                                                       config.isThrottlingEnabled ?? false,
                                                                       config.fidoServerUrl,
                                                                       false)
                }
                semaphore.signal()
            }
            semaphore.wait()
        }
    }
    
    private func checkConfigResponse(){
        if isValidConfigResponse() {
            NotificationCenter.default.post(name: CustomNotifications.apiLoadSuccess, object: nil)
        } else {
            NotificationCenter.default.post(name: CustomNotifications.apiLoadFailure, object: nil)
        }
    }
    
    private func getConfigurationResponse(completion: @escaping(Configuration?) -> ()) {
        let versionNumber = Bundle.main.object(forInfoDictionaryKey: "CFBundleShortVersionString") as! String
        
        let configurationPathWithAppendedVersion = String(format: config().ConfigurationApiPath, versionNumber)
        
        let requestUrl = URL(string: "\(config().BaseApiUrl)\(configurationPathWithAppendedVersion)")!
        let request = URLRequest(url:requestUrl)
        
        let task = URLSession.shared.dataTask(with: request) {
            (data, response, error) in
            if error == nil, let usableData = data {
                let decoder = JSONDecoder()
                do {
                    let appConfig = try decoder.decode(Configuration.self, from: usableData)
                    completion(appConfig)
                } catch {
                    if #available(iOS 10.0, *) {
                        os_log("Failure doing native app version http check: %@", log: OSLog.default, type: .error, "\(error)")
                    } else {
                        NSLog("Failure doing native app version http check: %@", "\(error)")
                    }
                    completion(nil)
                }
            } else {
                self.logFailure()
                completion(nil)
            }
        }
        task.resume()
    }
    
    func logFailure() {
        if #available(iOS 10.0, *) {
            os_log("Failure doing native app version http check", log: OSLog.default, type: .error)
        } else {
            NSLog("Failure doing native app version http check")
        }
    }
    
    func isUserDeviceAllowed(homeViewController: HomeViewController, completionHandler: @escaping (ConfigurationResponse?) -> Void) {
        if isValidConfigResponse() {
            if(configurationResponse.isThrottlingEnabled) {
                homeViewController.openThrottlingCarousel()
            }
            
            completionHandler(configurationResponse)
        } else {
            doConfigurationCallAndValidateResponse()
            completionHandler(nil)
        }
    }
    
    func isValidConfigResponse() -> Bool {
        return !configurationResponse.callFailed
    }
    
    func doConfigurationCallAndValidateResponse() {
        makeConfigCall()
        checkConfigResponse()
    }
    
    public func FidoServerUrl() -> String {
        return configurationResponse.FidoServerUrl
    }
    
    class func shared() -> ConfigurationService {
        return sharedConfigurationService
    }
    
    func getErrorMessage() -> ErrorMessage {
        let nhsOnlineErrorTitle = NSLocalizedString("ConnectionErrorTitle", comment: "")
        let nhsOnlineErrorMessage = NSLocalizedString("ConnectionErrorMessage", comment: "")
        let accessibleNhsOnlineErrorMessage = NSLocalizedString("AccessibilityConnectionErrorMessage", comment: "")
        
        return ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage, accessibleMessage: accessibleNhsOnlineErrorMessage)
    }
}

