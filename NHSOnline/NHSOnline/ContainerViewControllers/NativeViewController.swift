import UIKit
import os.log

class NativeViewController: UIViewController {
    
    @IBAction func reloadPageTrigger(_ sender: Any) {
        
        let parentVC = self.parent as? HomeViewController
        let webViewController = parentVC?.childViewControllers.first as? WebViewController

        webViewController?.webView.loadPage(url: (parentVC?.pageUrl)!)
    }
}
