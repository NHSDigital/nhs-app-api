import Foundation
import UIKit

class MailToSchemeHandler: BaseSchemeHandler {
    public var Scheme: String? { return "mailto" }
    func handle(url: URL) -> Bool {
        let uc = URLComponents(url: url, resolvingAgainstBaseURL: true)
        if(uc?.scheme != Scheme) {
            return false
        }
        
        UIApplication.shared.open(url)
        return true
    }
}
