@testable import NHSOnline

class LoggingServiceMocks: LoggingServiceProtocol {
    var calledLogError = false
    var calledLogInfo = false
    
    func logError(message: String) {
        calledLogError = true
    }
    
    func logInfo(message: String) {
        calledLogInfo = false
    }
}
