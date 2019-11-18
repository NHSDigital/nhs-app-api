import Foundation
import os.log

class FileReader: NSObject {

    func readContentFromLocation(fileLocation :String) -> String {
        let url = URL(fileURLWithPath:fileLocation)
        var content = ""
        
        do {
            content = try String(contentsOf: url, encoding: .utf8)
        } catch {
            os_log("Failed to load java script resource.", log: OSLog.default, type: .error)
        }
        
        return content
    }
}
