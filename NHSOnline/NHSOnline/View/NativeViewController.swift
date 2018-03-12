import UIKit
import os.log

class NativeViewController: UIViewController {
    
    @IBAction func reloadPageTrigger(_ sender: Any) {
        os_log("Trying to reload page on button click", log: OSLog.default, type: .info)
        
        let parentVC = self.parent as? BaseTabBarViewController
        let webViewController = parentVC?.childViewControllers.first as? WebViewController

        webViewController?.webView.loadPage(stringUrl: (parentVC?.pageUrl)!)
    }
}
