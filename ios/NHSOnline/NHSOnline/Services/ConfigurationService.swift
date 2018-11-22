import Foundation
import os.log

class ConfigurationResponse {
    public var isValidConfiguration = false
    public var isThrottlingEnabled = false
}

class ConfigurationService {
    private let homeViewController: HomeViewController
    
    init(homeViewController: HomeViewController) {
        self.homeViewController = homeViewController
    }
    
    func isUserDeviceAllowed(completionHandler: @escaping (ConfigurationResponse) -> Void) {
        let versionNumber = Bundle.main.object(forInfoDictionaryKey: "CFBundleShortVersionString") as! String
        
        let configurationPathWithAppendedVersion = String(format: config().ConfigurationApiPath, versionNumber)
        
        let requestUrl = URL(string: "\(config().BaseApiUrl)\(configurationPathWithAppendedVersion)")!
        let request = URLRequest(url:requestUrl)
        let task = URLSession.shared.dataTask(with: request) {
            (data, response, error) in
            if error == nil, let usableData = data {
                let decoder = JSONDecoder()
                do {
                    self.homeViewController.appVersionCheckError = false
                    let appConfig = try decoder.decode(Configuruation.self, from: usableData)
                    
                    let configurationResponse = ConfigurationResponse()
                    configurationResponse.isValidConfiguration = appConfig.isDeviceSupported
                    configurationResponse.isThrottlingEnabled = appConfig.isThrottlingEnabled
                    
                    if(configurationResponse.isThrottlingEnabled) {
                        self.homeViewController.openThrottlingCarousel()
                    }
                    
                    completionHandler(configurationResponse)
                }
                catch {
                    if #available(iOS 10.0, *) {
                        os_log("Failure doing native app version http check: %@", log: OSLog.default, type: .error, "\(error)")
                    } else {
                        NSLog("Failure doing native app version http check: %@", "\(error)")
                    }
                    
                    self.homeViewController.appVersionCheckError = true
                    DispatchQueue.main.async {
                        self.homeViewController.showNativeViewContainer(errorMessage: self.getErrorMessage())
                    }
                }
            } else {
                self.homeViewController.appVersionCheckError = true
                DispatchQueue.main.async {
                    self.homeViewController.showNativeViewContainer(errorMessage: self.getErrorMessage())
                }
            }
        }
        task.resume()
    }
    
    func getErrorMessage() -> ErrorMessage {
        let nhsOnlineErrorTitle = NSLocalizedString("ConnectionErrorTitle", comment: "")
        let nhsOnlineErrorMessage = NSLocalizedString("ConnectionErrorMessage", comment: "")
        let accessibleNhsOnlineErrorMessage = NSLocalizedString("AccessibilityConnectionErrorMessage", comment: "")
        
        return ErrorMessage(title: nhsOnlineErrorTitle, message: nhsOnlineErrorMessage, accessibleMessage: accessibleNhsOnlineErrorMessage)
    }
}
