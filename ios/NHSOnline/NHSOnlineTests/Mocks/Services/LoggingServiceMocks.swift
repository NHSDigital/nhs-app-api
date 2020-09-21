@testable import NHSOnline

class LoggingServiceMocks: LoggingServiceProtocol {
    var calledLogError = false
    var calledLogInfo = false
    var errorMessage = ""
    
    func logError(message: String) {
        calledLogError = true
        errorMessage = message
    }
    
    func logInfo(message: String) {
        calledLogInfo = false
    }
}
