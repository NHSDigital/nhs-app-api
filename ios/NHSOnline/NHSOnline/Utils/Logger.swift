import Foundation
import os.log

class Logger {
    static func logError(message: StaticString) {
        if #available(iOS 10.0, *) {
            os_log(message, log: OSLog.default, type: .error)
        } else {
            NSLog("\(message)")
        }
    }
    
    static func logInfo(message: StaticString) {
        if #available(iOS 10.0, *) {
            os_log(message, log: OSLog.default, type: .info)
        } else {
            NSLog("\(message)")
        }
    }
}


