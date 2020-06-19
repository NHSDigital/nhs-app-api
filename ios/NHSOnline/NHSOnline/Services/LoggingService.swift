import Foundation
import UIKit

class LoggingService: LoggingServiceProtocol {
    private var url: URL? = URL(string: config().BaseApiUrl + config().LoggerApiPath)
    
    func logError(message: String) {
        postLogEvent(message: message, level: .Error)
    }
    
    func logInfo(message: String) {
        postLogEvent(message: message, level: .Information)
    }
    
    private func postLogEvent(message: String, level: LogLevel) {
        let formattedMessage = String(format: "Platform:iOS-%@ %@", UIDevice.current.systemVersion, message)
        let logMessage = LoggingRequest(message: formattedMessage, level: level)
    
        var request = URLRequest(url: url!)

        let jsonData = try! JSONEncoder().encode(logMessage)
        let jsonString = String(data: jsonData, encoding: .utf8)!
        
        Logger.logInfo(message: "Posting to logger: %@" , jsonString)
        
        request.httpMethod = "POST"
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")
        request.httpBody = jsonData

        let task = URLSession.shared.dataTask(with: request) {
            (data, response, error) in
            guard error == nil else{
                Logger.logError(message: "Error posting to logger endpoint. Description: %@", error?.localizedDescription ?? "data")
                return
            }
            
            if let httpStatus = response as? HTTPURLResponse, httpStatus.statusCode != 200 {
                Logger.logError(message: "Error posting to logger endpoint. Status code: %d", httpStatus.statusCode)
                return
            }
        }
        task.resume()
    }
}
