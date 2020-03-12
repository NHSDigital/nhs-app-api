import Foundation
import os.log

class ConfigurationService {    
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
                Logger.logError(message: "Failure doing native app version http check", "\(String(describing: error))")
                completion(nil)
            }
        }
        task.resume()
    }
}
