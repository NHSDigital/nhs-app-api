import UIKit

class Safari {
    
    func open(url: URL) {
        UIApplication.shared.open(url, options: [:], completionHandler: { (status) in
        })
    }
}
