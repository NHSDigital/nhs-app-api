import UIKit
import os.log

class PageUnavailabilityViewController: UIViewController {
    @IBOutlet weak var errorTextView: ErrorTextView!
    @IBOutlet weak var tryAgainLabel: UILabel!
    var failedUrl: String? = nil
    
    @IBAction func reloadPageTrigger(_ sender: Any) {
        let parentVC = self.parent as? HomeViewController
        parentVC?.webViewController?.reloadWebView()
        let parentVCU = self.parent as? UnsecureViewController
        parentVCU?.webViewController?.reloadWebView()
    }
    
    override func viewDidAppear(_ animated: Bool) {
        errorTextView.layer.addTopBorder(color: UIColor.red, thickness: 1)
        errorTextView.resizeErrorTextView()
        self.configureNavBar()

    }
    
    func setUnavailabilityError(errorMessage:ErrorMessage) {
        if errorMessage.message != nil {
            self.tryAgainLabel.hideView()
        } else {
            self.tryAgainLabel.showView()
        }
        errorTextView.setServiceError(title: errorMessage.title, message: errorMessage.message)
        errorTextView.accessibilityValue = errorMessage.title + ". " + errorMessage.accessibleMessage!
    }
    
    func configureNavBar() {
        self.navigationItem.backBarButtonItem?.title = ""
    }
}
