import Foundation
import os.log

class Logger {
    static func logError(message: StaticString) {
        os_log(message, log: OSLog.default, type: .error)
    }
    
    static func logInfo(message: StaticString) {
        os_log(message, log: OSLog.default, type: .info)
    }
}


