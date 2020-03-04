import Foundation
import os.log

class ConfigurationService: ConfigurationServiceProtocol {
    private static var sharedConfigurationService = ConfigurationService()
    private var configurationResponse = ConfigurationResponse()
    
    private init() {
    }

    func getConfigurationResponse(completion: @escaping (Configuration?) -> ()) {
        let requestUrl = URL(string: config().BaseApiUrl + config().ConfigurationApiPath)!
        let request = URLRequest(url: requestUrl)

        let task = URLSession.shared.dataTask(with: request) {
            (data, response, error) in
            if error == nil, let usableData = data {
                let decoder = JSONDecoder()
                do {
                    let appConfig = try decoder.decode(Configuration.self, from: usableData)
                    completion(appConfig)
                } catch {
                    Logger.logError(message: "Failure doing native app version http check: %@", "\(error)")
                    completion(nil)
                }
            } else {
                Logger.logError(message: "Failure doing native app version http check")
                completion(nil)
            }
        }
        task.resume()
    }

    class func shared() -> ConfigurationService {
        sharedConfigurationService
    }
}
