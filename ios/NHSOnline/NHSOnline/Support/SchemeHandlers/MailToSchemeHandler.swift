import Foundation
import UIKit

class MailToSchemeHandler: BaseSchemeHandler {
    public var Scheme: String? { return "mailto" }
    func handle(url: URL) -> Bool {
        let uc = URLComponents(url: url, resolvingAgainstBaseURL: true)
        if(uc?.scheme != Scheme) {
            return false
        }
        
        if #available(iOS 10.0, *) {
            UIApplication.shared.open(url)
        } else {
            UIApplication.shared.openURL(url)
        }
        return true
    }
}
