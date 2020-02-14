import Foundation
import os.log

class Logger {
    static func logError(message: StaticString, _ args: CVarArg...) {
        if #available(iOS 10.0, *) {
            os_log(message, log: OSLog.default, type: .error, args)
        } else {
            NSLog("\(message)")
        }
    }
    
    static func logInfo(message: StaticString, _ args: CVarArg...) {
        if #available(iOS 10.0, *) {
            os_log(message, log: OSLog.default, type: .info, args)
        } else {
            NSLog("\(message)")
        }
    }
}
