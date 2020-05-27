import UIKit
import os.log

class PageUnavailabilityViewController: UIViewController {
    @IBOutlet weak var errorIconTextView: UITextView!
    @IBOutlet weak var errorTextView: ErrorTextView!
    @IBOutlet weak var tryAgainButton: UIButton!
    
    var failedUrl: String? = nil
    
    @IBAction func reloadPageTrigger(_ sender: Any) {
        let parentVC = self.parent as? HomeViewController
        let name = CustomNotifications.pageUnavailabilityOnReloadWebView
        NotificationCenter.default.post(name: name, object: self)
        
        parentVC?.webViewController?.reloadWebView()
    }
    
    override var preferredStatusBarStyle: UIStatusBarStyle {
        return .lightContent
    }
    
    override func viewDidAppear(_ animated: Bool) {
        errorTextView.layer.addTopBorder(color: UIColor(red:0.85, green:0.16, blue:0.11, alpha:1.0), thickness: 3)
        errorTextView.resizeErrorTextView()
    }
    
    override func viewDidLoad() {
        self.setNeedsStatusBarAppearanceUpdate()
        super.viewDidLoad()
        
    }
    
    func setUnavailabilityError(errorMessage:ErrorMessage) {
        errorTextView.setServiceError(title: errorMessage.title, message: errorMessage.message)
        var accessibilityText = errorMessage.title
        if let extraAccessibilityText = errorMessage.accessibleMessage {
            accessibilityText.append(". " + extraAccessibilityText)
        }
        errorTextView.accessibilityValue = accessibilityText
    }
    
    func setTryAgainButtonText(text: String) {
        self.tryAgainButton.setTitle(text, for: .normal)
    }
}
