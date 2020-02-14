import Foundation
import os.log

class FileReader: NSObject {

    func readContentFromLocation(fileLocation :String) -> String {
        let url = URL(fileURLWithPath:fileLocation)
        var content = ""
        
        do {
            content = try String(contentsOf: url, encoding: .utf8)
        } catch {
            Logger.logError(message: "Failed to load java script resource")
        }
        
        return content
    }
}
