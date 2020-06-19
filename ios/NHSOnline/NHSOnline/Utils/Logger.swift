import Foundation
import os.log

class Logger {
    static func logError(message: String, _ args: CVarArg...) {
        let formattedMessage = String(format: message, arguments: args)
        
        if #available(iOS 10.0, *) {
            os_log("%@", type: OSLogType.debug, formattedMessage)
        } else {
            NSLog(formattedMessage)
        }
    }
    
    static func logInfo(message: String, _ args: CVarArg...) {
        let formattedMessage = String(format: message, arguments: args)
        
        if #available(iOS 10.0, *) {
            os_log("%@", type: OSLogType.info, formattedMessage)
        } else {
            NSLog(formattedMessage)
        }
    }
}
