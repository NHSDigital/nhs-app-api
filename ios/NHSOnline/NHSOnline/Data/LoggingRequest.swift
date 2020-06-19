import Foundation

struct LoggingRequest: Codable {
    var level: String
    var message: String
    var timeStamp: String
    
    init(
        message: String,
        level: LogLevel
    ) {
        self.level = level.rawValue
        self.message = message
        self.timeStamp = Date().iso8601
    }
}
