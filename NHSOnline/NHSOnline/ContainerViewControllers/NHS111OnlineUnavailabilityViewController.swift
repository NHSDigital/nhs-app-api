import UIKit
import os.log

class NHS111OnlineUnavailabilityViewController: UIViewController {
    
    @IBAction func reloadPageTrigger(_ sender: Any) {
        
        let parentVC = self.parent as? HomeViewController
        let webViewController = parentVC?.childViewControllers.first as? WebViewController

        webViewController?.webView.loadPage(url: (parentVC?.nhs111Url)!)
    }
}
