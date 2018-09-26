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
        var accessibilityText = errorMessage.title
        if let extraAccessibilityText = errorMessage.accessibleMessage {
            accessibilityText.append(". " + extraAccessibilityText)
        }
        errorTextView.accessibilityValue = accessibilityText
    }
    
    func configureNavBar() {
        self.navigationItem.backBarButtonItem?.title = ""
    }
}
