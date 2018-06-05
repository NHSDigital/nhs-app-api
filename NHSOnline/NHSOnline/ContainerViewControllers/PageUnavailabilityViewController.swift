import UIKit
import os.log

class PageUnavailabilityViewController: UIViewController {
    @IBOutlet weak var errorTextView: ErrorTextView!
    @IBOutlet weak var tryAgainLabel: UILabel!
    var failedUrl: String? = nil
    
    @IBAction func reloadPageTrigger(_ sender: Any) {
        let parentVC = self.parent as? HomeViewController
        parentVC?.webViewController?.reloadWebView()
    }
    
    override func viewDidAppear(_ animated: Bool) {
        errorTextView.layer.addTopBorder(color: UIColor.red, thickness: 1)
    }
    
    func setUnavailabilityError(errorMessage:ErrorMessage) {
        if errorMessage.message != nil {
            self.tryAgainLabel.isHidden = true
        } else {
            self.tryAgainLabel.isHidden = false
        }
        errorTextView.setServiceError(title: errorMessage.title, message: errorMessage.message)
    }
}
