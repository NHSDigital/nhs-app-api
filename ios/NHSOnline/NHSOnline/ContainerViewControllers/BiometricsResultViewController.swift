import UIKit
import Foundation

class BiometricsResultViewController: UIViewController {
    @IBOutlet weak var BoxTitle: UITextView!
    @IBOutlet weak var BoxText: ErrorTextView!
    @IBOutlet weak var BackButton: UIButton!
    
    var viewController: HomeViewController?
    var registration: Bool = true
    
    override func viewDidAppear(_ animated: Bool) {
        setupViewController()
    }
    
    override func viewDidLoad() {
        self.setNeedsStatusBarAppearanceUpdate()
        super.viewDidLoad()
    }
    
    override var preferredStatusBarStyle: UIStatusBarStyle {
        return .lightContent
    }
    
    func setupViewController(){
        let stringsObject = BiometricStringHandler().getResultStrings(isRegistration: registration)
        viewController!.updateHeaderText(headerText: stringsObject.HeaderTextText)
        BoxText.text = stringsObject.BoxTextText
        BackButton.setTitle(NSLocalizedString("BiometricBackButtonText", comment: ""), for: .normal)
        BackButton.titleLabel?.adjustsFontSizeToFitWidth = true
        BoxText.resizeErrorTextView()
    }
    
    @IBAction func backToMyAccount(_ sender: Any) {
        UserDefaults.standard.set(config().HelpAccountURL, forKey: "HelpUrl")
        viewController?.showWebViewContainer()
    }
}
