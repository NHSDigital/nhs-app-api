import UIKit
import os.log

class PageUnavailabilityViewController: UIViewController {
    @IBOutlet weak var errorLabelWrapper: UIView!
    
    @IBAction func reloadPageTrigger(_ sender: Any) {
        let parentVC = self.parent as? HomeViewController
        let webViewController = parentVC?.childViewControllers.first as? WebViewController

        webViewController?.webView.loadPage(url: (parentVC?.pageUrl)!)
    }
    
    override func viewDidAppear(_ animated: Bool) {
        errorLabelWrapper.layer.addTopBorder(color: UIColor.red, thickness: 1)
    }
}
